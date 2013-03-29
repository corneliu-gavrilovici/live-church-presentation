using System.IO;
using System.Reflection;
using System.Xml;

namespace LiveBiblePresentation.Data
{
    public static class Globals
    {
        public const string BIBLES_FOLDER_NAME = "Bibles";
        public const string XML_FILE_FORMAT = "{0}.xml";

        /// <summary>
        /// Attributes the inner text.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string AttributeInnerText(this XmlNode node, string name)
        {
            XmlAttribute attr = node.Attributes[name];

            if (attr != null)
                return attr.InnerText;

            return null;
        }

        /// <summary>
        /// Toes the integer.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="defaultValue">The default value in case of an exception.</param>
        /// <returns></returns>
        public static bool TryParseToInteger(this string value, out int result, int defaultValue = default(int))
        {
            result = defaultValue;

            return int.TryParse(value, out result);
        }

        /// <summary>
        /// Tries the parse to integer attribute.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseAttributeToInteger(this XmlNode node, string attribute, out int result)
        {
            result = default(int);

            if (node.Attributes[attribute] != null)
            {
                return node.Attributes[attribute].InnerText.TryParseToInteger(out result);
            }

            return false;
        }

        public static string GetBibleFilePath(BibleLanguage bibleLanguage)
        {
            return Path.Combine(BiblesDirPath, string.Format(XML_FILE_FORMAT, bibleLanguage.ToString()));
        }

        public static string AppPath
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }

        public static string BiblesDirPath
        {
            get { return Path.Combine(AppPath, BIBLES_FOLDER_NAME); }
        }
    }
}