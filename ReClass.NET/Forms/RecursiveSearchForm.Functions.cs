using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.MemoryScanner;
using ReClassNET.MemoryScanner.Comparer;
using ReClassNET.Nodes;
using ReClassNET.UI;

namespace ReClassNET.Forms
{
	public partial class RecursiveSearchForm
	{
		private ClassNode CastToClassOrGetParentClass(BaseNode baseNode)
		{
			ClassNode classNode;

			if (baseNode is ClassNode node)
			{
				classNode = node;
			}
			else
			{
				classNode = baseNode.GetParentClass();
			}

			return classNode;
		}

		private void ExtendClassToFitOffset(ClassNode classNode, uint offset, uint size) // + sizeof(searchType)
		{
			var bytesToAdd = (int)(offset + size - classNode.MemorySize);
			classNode.AddBytes(bytesToAdd > 0 ? bytesToAdd : 0);
		}

		private PointerNode CreatePointer()
		{
			var classNode = ClassNode.Create();
			var classInstanceNode = new ClassInstanceNode();
			classInstanceNode.ChangeInnerNode(classNode);
			var pointerNode = new PointerNode();
			pointerNode.ChangeInnerNode(classInstanceNode);
			return pointerNode;
		}

		private ClassNode GetPointerChildClass(PointerNode pointerNode)
		{
			if (pointerNode.InnerNode is ClassInstanceNode classInstanceNode)
			{
				var classNode = classInstanceNode.InnerNode as ClassNode;
				return classNode;
			}

			return null;
		}

		private readonly Stack<Tuple<IntPtr, uint>> addressAndOffsetStack = new();

		private class SearchResult
		{
			public SearchResult(SearchType searchType, IntPtr baseAddress, IntPtr address, List<uint> offsets)
			{
				this.SearchType = searchType;
				this.BaseAddress = baseAddress;
				this.Address = address;
				this.Offsets = offsets;
			}

			public SearchType SearchType;
			public IntPtr BaseAddress;
			public IntPtr Address;
			public List<uint> Offsets;
		}

		private readonly List<SearchResult> searchResults = new();
		private void CopySearchResultsToClipboard(RemoteProcess process, SearchType searchType)
		{
			var r = "";
			foreach (var i in searchResults)
			{
				r += "0x" + i.BaseAddress.ToString("X");
				foreach (var j in i.Offsets)
				{
					r += " 0x" + j.ToString("X");
				}

				r += "\n";
			}

			Clipboard.SetText(r);
		}

		private void StartMemoryScanFromSearchResults(RemoteProcess process, SearchType searchType)
		{
			var sf = GlobalWindowManager.Windows.OfType<ScannerForm>().FirstOrDefault();
			if (sf != null)
			{
				if (MessageBox.Show("Open a new scanner window?", Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					sf = null;
				}
			}
			if (sf == null)
			{
				sf = new ScannerForm(Program.RemoteProcess);
				sf.Show();
			}

			var scanResults = new List<ScanResult>();
			var scanSettings = ScanSettings.Default;
			foreach (var i in searchResults)
			{
				ScanResult t;
				switch (searchType)
				{
					case SearchType.Hex32Node:
						t = new IntegerScanResult(
								process.BitConverter.ToInt32(
									process.ReadRemoteMemory(
										i.Address, 4), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Integer;
						break;
					case SearchType.Hex16Node:
						t = new IntegerScanResult(
								process.BitConverter.ToInt16(
									process.ReadRemoteMemory(
										i.Address, 2), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Short;
						break;
					case SearchType.Int32Node:
						t = new IntegerScanResult(
								process.BitConverter.ToInt32(
									process.ReadRemoteMemory(
										i.Address, 4), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Integer;
						break;
					case SearchType.Int16Node:
						t = new IntegerScanResult(
								process.BitConverter.ToInt16(
									process.ReadRemoteMemory(
										i.Address, 2), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Short;
						break;
					case SearchType.BoolNode:
						t = new ByteScanResult(
							Convert.ToByte(process.BitConverter.ToBoolean(
								process.ReadRemoteMemory(
									i.Address, 1), 0)));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Byte;
						break;
					case SearchType.FloatNode:
						t = new FloatScanResult(
								process.BitConverter.ToSingle(
									process.ReadRemoteMemory(
										i.Address, sizeof(float)), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Float;
						break;
					case SearchType.DoubleNode:
						t = new DoubleScanResult(
								process.BitConverter.ToDouble(
									process.ReadRemoteMemory(
										i.Address, sizeof(double)), 0));
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.Double;
						break;
					case SearchType.Utf8TextPtrNode:
						t = new StringScanResult(
							process.ReadRemoteString(
								i.Address, Encoding.UTF8, 1024), Encoding.UTF8);
						t.Address = i.Address;
						scanResults.Add(t);
						scanSettings.ValueType = ScanValueType.String;
						break;
				}
			}

			sf.SetGuiFromSettings(scanSettings);
			
			sf.GetAdressListMemoryRecordList().SetRecords(scanResults.Select(r =>
			{
				var record = new MemoryRecord(r);
				record.ResolveAddress(process);
				return record;
			}));

			sf.Show();
		}

		private void AddNodesFromSearchResults(BaseNode rootNode)
		{
			var whereToInsertClass = CastToClassOrGetParentClass(rootNode);

			foreach (var searchResult in searchResults)
			{
				if (searchResult.Offsets.Count <= 0)
				{
					continue;
				}

				foreach (var offset in searchResult.Offsets)
				{
					var alreadyExistingAtOffset = whereToInsertClass.Nodes.FirstOrDefault(x => x.Offset == offset);

					if (offset.Equals(searchResult.Offsets.Last()))
					{
						if (alreadyExistingAtOffset == null)
						{
							ExtendClassToFitOffset(whereToInsertClass, offset, (uint)gridNumericUpDown.Value);
						}

						BaseNode node;
						switch (searchResult.SearchType)
						{
							case SearchType.Hex32Node:
								node = new Hex32Node();
								break;
							case SearchType.Hex16Node:
								node = new Hex16Node();
								break;
							case SearchType.Int32Node:
								node = new Int32Node();
								break;
							case SearchType.Int16Node:
								node = new Int16Node();
								break;
							case SearchType.BoolNode:
								node = new BoolNode();
								break;
							case SearchType.FloatNode:
								node = new FloatNode();
								break;
							case SearchType.DoubleNode:
								node = new DoubleNode();
								break;
							case SearchType.Utf8TextPtrNode:
								node = new Utf8TextPtrNode();
								break;
							default:
								node = new Int32Node();
								break;
						}

						node.Comment = "<== FOUND";

						var nodeToReplace = whereToInsertClass.Nodes.FirstOrDefault(x => x.Offset == offset);
						whereToInsertClass.InsertNode(nodeToReplace, node);
						whereToInsertClass.RemoveNode(nodeToReplace);

						whereToInsertClass = CastToClassOrGetParentClass(rootNode);
					}
					else
					{
						if (alreadyExistingAtOffset == null)
						{
							ExtendClassToFitOffset(whereToInsertClass, offset, (uint)gridNumericUpDown.Value);
						}

						ClassNode pointerNodeChildClass;

						if (alreadyExistingAtOffset is PointerNode existingAtOffsetPointerNode)
						{
							pointerNodeChildClass = GetPointerChildClass(existingAtOffsetPointerNode);
						}
						else
						{
							var pointerNode = CreatePointer();

							var nodeToReplace = whereToInsertClass.Nodes.FirstOrDefault(x => x.Offset == offset);
							whereToInsertClass.InsertNode(nodeToReplace, pointerNode);
							whereToInsertClass.RemoveNode(nodeToReplace);

							pointerNodeChildClass = GetPointerChildClass(pointerNode);
						}

						whereToInsertClass = pointerNodeChildClass;
					}
				}
			}
		}

		private void SearchOffset(
			RemoteProcess process,
			IntPtr baseAddress,
			Stack<Tuple<IntPtr, uint>> addressAndOffsets,
			SearchType searchType)
		{
			switch (searchType)
			{
				case SearchType.Hex32Node:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToUInt32(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(uint)), 0);
						var compareValue = uint.TryParse(
							valueTextBox.Text,
							System.Globalization.NumberStyles.HexNumber,
							System.Threading.Thread.CurrentThread.CurrentCulture,
							out var compareValueResult);

						if (compareValue && value == compareValueResult)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.Hex32Node,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.Hex16Node:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToUInt16(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(ushort)), 0);
						var compareValue = ushort.TryParse(
							valueTextBox.Text,
							System.Globalization.NumberStyles.HexNumber,
							System.Threading.Thread.CurrentThread.CurrentCulture,
							out var compareValueResult);

						if (compareValue && value == compareValueResult)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.Hex16Node,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.Int32Node:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToInt32(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(int)), 0);

						if (value.ToString() == valueTextBox.Text)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.Int32Node,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.Int16Node:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToInt16(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(short)), 0);

						if (value.ToString() == valueTextBox.Text)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.Int16Node,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.BoolNode:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToBoolean(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(bool)), 0);

						if (value.ToString() == valueTextBox.Text)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.BoolNode,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.FloatNode:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToSingle(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(float)), 0);
						var compareValue = float.TryParse(valueTextBox.Text, out var compareValueResult);

						if (compareValue && Math.Abs(value - compareValueResult) < 0.5f)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.FloatNode,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.DoubleNode:
					{
						var t = addressAndOffsets.Peek();
						var value = process.BitConverter.ToDouble(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, sizeof(double)), 0);
						var compareValue = double.TryParse(valueTextBox.Text, out var compareValueResult);

						if (compareValue && Math.Abs(value - compareValueResult) < 0.5)
						{
							var offsetPathFound = new List<uint>();
							foreach (var i in addressAndOffsets.Reverse())
							{
								offsetPathFound.Add(i.Item2);
							}

							searchResults.Add(new SearchResult(
								SearchType.DoubleNode,
								baseAddress,
								t.Item1 + (int)t.Item2,
								offsetPathFound));
						}
					}
					break;
				case SearchType.Utf8TextPtrNode:
					{
						var t = addressAndOffsets.Peek();

						var addressPtr = (IntPtr)process.BitConverter.ToInt32(
							process.ReadRemoteMemory(
								t.Item1 + (int)t.Item2, 4), 0);

						var addressPtrData = new byte[4];
						if (process.ReadRemoteMemoryIntoBuffer(addressPtr, ref addressPtrData))
						{
							var value = process.ReadRemoteString(addressPtr, Encoding.UTF8, 1024);
							if (value == valueTextBox.Text)
							{
								var offsetPathFound = new List<uint>();
								foreach (var i in addressAndOffsets.Reverse())
								{
									offsetPathFound.Add(i.Item2);
								}

								searchResults.Add(new SearchResult(
									SearchType.Utf8TextPtrNode,
									baseAddress,
									addressPtr,
									offsetPathFound));
							}
						}
					}
					break;
			}
		}

		private void SearchRecurse(RemoteProcess process, IntPtr address, SearchType searchType)
		{
			var baseAddress = address;
			addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(address, 0));
			while (true)
			{
				var topAddress = addressAndOffsetStack.Peek();
				var addressPtr = (IntPtr)process.BitConverter.ToInt32(
					process.ReadRemoteMemory(
						topAddress.Item1 + (int)topAddress.Item2, 4), 0);

				var addressPtrData = new byte[4];
				if (process.ReadRemoteMemoryIntoBuffer(addressPtr, ref addressPtrData))
				{
					// Valid pointer

					if (searchType == SearchType.Utf8TextPtrNode)
					{
						SearchOffset(process, baseAddress, addressAndOffsetStack, searchType);
					}

					if (addressAndOffsetStack.Count < levelNumericUpDown.Value)
					{
						addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(addressPtr, 0));
					}
					else
					{
						var t = addressAndOffsetStack.Pop();
						addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(t.Item1, t.Item2 + (uint)gridNumericUpDown.Value));
						if (t.Item2 + (uint)gridNumericUpDown.Value >= offsetNumericUpDown.Value)
						{
							addressAndOffsetStack.Pop();
							if (addressAndOffsetStack.Count < 1) return;
							t = addressAndOffsetStack.Pop();
							addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(t.Item1, t.Item2 + (uint)gridNumericUpDown.Value));
						}
					}
				}
				else
				{
					// Not a valid pointer
					SearchOffset(process, baseAddress, addressAndOffsetStack, searchType);
					var t = addressAndOffsetStack.Pop();
					addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(t.Item1, t.Item2 + (uint)gridNumericUpDown.Value));
					if (t.Item2 + (uint)gridNumericUpDown.Value >= offsetNumericUpDown.Value)
					{
						addressAndOffsetStack.Pop();
						if (addressAndOffsetStack.Count < 1) return;
						t = addressAndOffsetStack.Pop();
						addressAndOffsetStack.Push(new Tuple<IntPtr, uint>(t.Item1, t.Item2 + (uint)gridNumericUpDown.Value));
					}
				}
			}
		}

		private void Search()
		{
			var selectedNode = Program.MainForm.GetMemoryViewControl().GetSelectedNodes().FirstOrDefault();
			if (selectedNode == null)
			{
				return;
			}

			Enum.TryParse<SearchType>(typeComboBox.SelectedValue.ToString(), out var searchType);

			if (selectedNode.Node is not ClassNode node)
			{
				return;
			}

			var process = selectedNode.Process;
			var address = process.ParseAddress(node.AddressFormula) + node.Offset;

			searchResults.Clear();

			SearchRecurse(process, address, searchType);

			MessageBox.Show("Found " + searchResults.Count + " results.");

			if (searchResults.Count > 0)
			{
				var dr = MessageBox.Show(
					"Add nodes?",
					"Search results",
					MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes)
				{
					AddNodesFromSearchResults(node);
				}

				dr = MessageBox.Show(
					"Copy results to clipboard?",
					"Search results",
					MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes)
				{
					CopySearchResultsToClipboard(process, searchType);
				}

				dr = MessageBox.Show(
					"Open in memory scanner?",
					"Search results",
					MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes)
				{
					StartMemoryScanFromSearchResults(process, searchType);
				}
			}

			//foreach (var searchResult in searchResults)
			//{
			//	if (searchResult.Count > 0)
			//	{
			//		var str = "Found at: ";
			//		foreach (var offset in searchResult)
			//		{
			//			str += "0x" + offset.ToString("X") + " ";
			//		}

			//		MessageBox.Show(str);
			//	}
			//}
		}
	}
}
