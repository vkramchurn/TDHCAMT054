using CAMT054.Process.Models;
using CAMT054.Process.Models.V04;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace CAMT054.Process
{
    public class ProcessXML
    {
        private string _xmlContent { get; set; }
        private string _xsd4Path { get; set; }
        private string _xsd8Path { get; set; }
        private string _namespace { get; set;}
        private string _aesKey { get; set; }
        private string _aesIv { get; set; }
        public ProcessXML(string xmlContent,string xsd4Path,string xsd8Path="",string aesKey="",string aesIv="")
        {
            XmlRequest req = JsonSerializer.Deserialize<XmlRequest>(xmlContent);
            _xmlContent = Utils.DecodeFromBase64(req.Content,aesKey,aesIv);
            _xsd4Path = xsd4Path;
            _xsd8Path = xsd8Path;
            _namespace = Utils.GetNamespace(_xmlContent);
            _aesKey = aesKey;
            _aesIv = aesIv;
        }
        public IList<BankTransactionDto> FetchCompiledResult() {

            try
            {
                ValidateAndFixXml();

                BankTransactionHelperV4 bankTransactionHelperV4 = new BankTransactionHelperV4(_xmlContent, _namespace);
                BankTransactionHelperV8 bankTransactionHelperV8 = new BankTransactionHelperV8(_xmlContent, _namespace);

                return _namespace switch
                {
                    "urn:iso:std:iso:20022:tech:xsd:camt.054.001.04"
                        => bankTransactionHelperV4.GetBankTransaction<CAMT054.Process.Models.V04.Document>(),

                    "urn:iso:std:iso:20022:tech:xsd:camt.054.001.08"
                        => bankTransactionHelperV8.GetBankTransaction<CAMT054.Process.Models.V08.Document>(),

                    _ => new List<BankTransactionDto>()
                };
            }
            catch (Exception ex)
            {
                // TODO: log properly
                throw new Exception(ex.InnerException.Message, ex);
            }

        }
        private void ValidateAndFixXml()
        {
            FixXmlBeforeValidation();

            ValidateXml();
        }
        private void FixXmlBeforeValidation()
        {
            StringBuilder output = new StringBuilder();
            _xmlContent = FixDateTimeIssue();
            
            using (XmlReader reader = XmlReader.Create(new StringReader(_xmlContent)))
            using (XmlWriter writer = XmlWriter.Create(output))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);

                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    writer.WriteAttributeString(
                                        reader.Prefix,
                                        reader.LocalName,
                                        reader.NamespaceURI,
                                        reader.Value
                                    );
                                }

                                reader.MoveToElement();
                            }
                            break;

                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            writer.WriteString(reader.Value);
                            break;

                        case XmlNodeType.EndElement:
                            writer.WriteEndElement();
                            break;
                    }
                }
            }

            _xmlContent = output.ToString();
        }
        private string FixDateTimeIssue()
        {
            string xmlContent = Regex.Replace(
                _xmlContent,
                @"(\d{4}-\d{2}-\d{2})T24:00:00(\+\d{2}:\d{2}|Z)?",
                m =>
                {
                    var date = DateTime.ParseExact(
                        m.Groups[1].Value,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);

                    date = date.AddDays(1);

                    var tz = m.Groups[2].Value;

                    return $"{date:yyyy-MM-dd}T00:00:00{m.Groups[2].Value}";
                });
            return xmlContent;
        }
        private bool CheckIfDateTimeIso(string val,string format)
        {
            var match = Regex.Match(val,@""+format);
            return match.Success;
        }

        private void ValidateXml()
        {
            List<string> errors = new List<string>();

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(
                _namespace,
                GetXSDPath()
            );

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            settings.ValidationEventHandler += (sender, e) =>
            {
                errors.Add($"{e.Severity}: {e.Message} at {e.Exception?.LineNumber},{e.Exception?.LinePosition}");
            };

            using (StringReader stringReader = new StringReader(_xmlContent))
            using (XmlReader reader = XmlReader.Create(stringReader, settings))
            {
                while (reader.Read()) { }
            }
            if (errors.Any())
            {
                throw new Exception("XML validation failed: " + string.Join("; ", errors));
            }
        }
        private string GetXSDPath() {
            var xsdpath = "";
            switch (_namespace)
            {
                case "urn:iso:std:iso:20022:tech:xsd:camt.054.001.04":
                    xsdpath = _xsd4Path;
                    break;
                case "urn:iso:std:iso:20022:tech:xsd:camt.054.001.08":
                    xsdpath = _xsd8Path;
                    break;
                default: break;
            }
            return xsdpath;
        }
    }
}
