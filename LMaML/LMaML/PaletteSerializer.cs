using System;
using System.Globalization;
using System.Windows.Media;
using System.Xml;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Serialization.Xml;

namespace LMaML
{
    public class PaletteSerializer : XmlSerializerBase<LinearGradientPalette>
    {
        private readonly IXmlSerializer<double> doubleSerializer;
        private readonly IXmlSerializer<Color> colourSerializer;

        public PaletteSerializer()
        {
            doubleSerializer = XmlSerializerService.GetSerializer<double>();
            colourSerializer = XmlSerializerService.GetSerializer<Color>();
        }

        public override void Serialize(LinearGradientPalette item, XmlWriter writer)
        {
            var map = item.GetMap();
            writer.WriteStartElement("Palette");
            try
            {
                writer.WriteAttributeString("Count", map.Length.ToString(CultureInfo.InvariantCulture));
                foreach (var element in map)
                {
                    doubleSerializer.Serialize(element.Item1, writer);
                    colourSerializer.Serialize(element.Item2, writer);
                }
            }
            finally { writer.WriteEndElement(); }
        }

        public override LinearGradientPalette Deserialize(XmlReader reader)
        {
            reader.SkipToElement("Palette");
            var result = new LinearGradientPalette();
            try
            {
                var countAttrib = reader.GetAttribute("Count");
                int count;
                if (!int.TryParse(countAttrib, out count)) return null;
                var target = new Tuple<double, Color>[count];
                if (reader.IsEmptyElement) return result;
                reader.ReadStartElement("Palette");
                for (var i = 0; i < count; ++i)
                {
                    var item1 = doubleSerializer.Deserialize(reader);
                    var item2 = colourSerializer.Deserialize(reader);
                    target[i] = new Tuple<double, Color>(item1, item2);
                }
                result.FromMap(target);
                return result;
            }
            finally
            {
                if (reader.IsEmptyElement)
                    reader.Skip();
                else
                    reader.ReadEndElement();
            }
        }
    }
}