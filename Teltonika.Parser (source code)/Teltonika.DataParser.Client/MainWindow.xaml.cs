using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Teltonika.DataParser.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DecodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = Regex.Replace(new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd).Text ,@"\t|\n|\r|\s", "");
            TextInput.Document.Blocks.Clear();
            TextInput.Document.Blocks.Add(new Paragraph(new Run(text.ToUpper())));

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Insert data.");
                return;
            }

            try
            {
                var bytes = StringToBytes(text);

                object data;
                if (TcpRadioButton.IsChecked != null && (bool) TcpRadioButton.IsChecked)
                {
                    data = DecodeTcpData(bytes);
                }
                else
                {
                    data = DecodeUdpData(bytes);
                }

                TreeView.Items.Clear();
                TreeView.Items.Add(data);
            }
            catch (Exception)
            {
                TreeView.Items.Clear();
                MessageBox.Show("Corrupted data inserted.");
            }
        }

        private static Data DecodeUdpData(byte[] bytes)
        {
            var parser = new DataReader(bytes);

            var datalist = new List<Data>
            {
                parser.ReadData(2, DataType.Length),
                parser.ReadData(2, DataType.PacketId),
                parser.ReadData(1, DataType.PacketType),
                parser.ReadData(1, DataType.AvlPacketId)
            };
            var imeiLength = parser.ReadData(2, DataType.ImeiLength);
            datalist.Add(imeiLength);
            datalist.Add(parser.ReadData((byte)(int.Parse(imeiLength.Value)), DataType.Imei));
            datalist.Add(DecodeAvlData(parser));

            var result = new Data("UDP AVL Data Packet")
            {
                SubItems = datalist.ToArray()
            };

            return result;
        }

        private static Data DecodeTcpData(byte[] bytes)
        {
            var parser = new DataReader(bytes);

            var dataList = new []
            {
                parser.ReadData(4, DataType.Preamble),
                parser.ReadData(4, DataType.AvlDataArrayLength),
                DecodeAvlData(parser),
                parser.ReadData(4, DataType.Crc)
            };

            var result = new Data("TCP AVL Data Packet")
            {
                SubItems = dataList
            };

            return result;
        }

        private static byte[] StringToBytes(string data)
        {
            var array = new byte[data.Length / 2];

            var substring = 0;
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = Byte.Parse(data.Substring(substring, 2), NumberStyles.AllowHexSpecifier);
                substring += 2;
            }

            return array;
        }

        private static Data DecodeAvlData(DataReader parser)
        {
            var codecIdData = parser.ReadData(1, DataType.CodecId);
            var countData = parser.ReadData(1, DataType.AvlDataCount);

            var avlDataList = new List<Data> { codecIdData, countData };
            var result = new Data("Data");

            for (var a = 0; a < Int32.Parse(countData.Value); a++)
            {
                var items = new PacketDecoder(parser, codecIdData.Value);

                // Fake data to show nodes in tree
                var startArraySegment = items.AvlItems.First().ArraySegment;
                var avlData = new Data("AVL Data")
                {
                    ArraySegment =
                        new ArraySegment<byte>(startArraySegment.Array, startArraySegment.Offset,
                            items.IoElementSubItems.Last().ArraySegment.Offset - startArraySegment.Offset + 1)
                };

                var gpsElement = new Data("GPS Element")
                {
                    ArraySegment =
                        new ArraySegment<byte>(startArraySegment.Array, items.GpsElementSubItems.First().ArraySegment.Offset, 15)
                };

                var ioElement = new Data("I/O Element")
                {
                    ArraySegment =
                        new ArraySegment<byte>(startArraySegment.Array, items.IoElementSubItems.First().ArraySegment.Offset,
                            items.IoElementSubItems.Last().ArraySegment.Offset - items.IoElementSubItems.First().ArraySegment.Offset + 1)
                };

                gpsElement.SubItems = items.GpsElementSubItems.ToArray();
                ioElement.SubItems = items.IoElementSubItems.ToArray();

                var avlDataSubItems = new List<Data>();
                avlDataSubItems.AddRange(items.AvlItems);
                avlDataSubItems.Add(gpsElement);
                avlDataSubItems.Add(ioElement);

                avlData.SubItems = avlDataSubItems.ToArray();
                avlDataList.Add(avlData);
            }
            avlDataList.Add(parser.ReadData(1, DataType.AvlDataCount));
            result.SubItems = avlDataList.ToArray();
            result.ArraySegment = new ArraySegment<byte>(codecIdData.ArraySegment.Array, codecIdData.ArraySegment.Offset,
                avlDataList.Last().ArraySegment.Offset - codecIdData.ArraySegment.Offset + 1);

            return result;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var data = e.NewValue as Data;
            if (data == null)
            {
                return;
            }

            var text = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
            text.ClearAllProperties();

            var startPosition = data.ArraySegment.Offset * 2;
            var endPosition = startPosition + data.ArraySegment.Count * 2;

            var start = text.Start.GetPositionAtOffset(startPosition);
            var end = text.Start.GetPositionAtOffset(endPosition);

            var range = new TextRange(start, end);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        }
    }
}
