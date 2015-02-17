using System.Globalization;
using System.Windows;
using System.Xml;
using iLynx.Serialization.Xml;

namespace LMaML
{
    public class RectSerializer : XmlSerializerBase<Rect>
    {
        public override void Serialize(Rect item, XmlWriter writer)
        {
            writer.WriteElementString(typeof(Rect).Name,
                string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", item.Left, item.Top, item.Width, item.Height));
        }

        public override Rect Deserialize(XmlReader reader)
        {
            var values = reader.ReadElementString(typeof (Rect).Name).Split(',');
            var x = double.Parse(values[0], CultureInfo.InvariantCulture);
            var y = double.Parse(values[1], CultureInfo.InvariantCulture);
            var width = double.Parse(values[2], CultureInfo.InvariantCulture);
            var height = double.Parse(values[3], CultureInfo.InvariantCulture);
            return new Rect(x, y, width, height);
        }
    }
}