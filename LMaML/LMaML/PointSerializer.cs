using System.Globalization;
using System.Windows;
using System.Xml;
using iLynx.Serialization.Xml;

namespace LMaML
{
    public class PointSerializer : XmlSerializerBase<Point>
    {
        public override void Serialize(Point item, XmlWriter writer)
        {
            writer.WriteElementString(typeof(Point).Name,
                string.Format(CultureInfo.InvariantCulture, "{0},{1}", item.X, item.Y));
        }

        public override Point Deserialize(XmlReader reader)
        {
            var values = reader.ReadElementString(typeof (Point).Name).Split(',');
            return new Point(double.Parse(values[0], CultureInfo.InvariantCulture), double.Parse(values[1], CultureInfo.InvariantCulture));
        }
    }
}