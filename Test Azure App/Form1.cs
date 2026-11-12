using CAMT054.Process;
using CAMT054.Process.Models;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace Test_Azure_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Select an XML file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string xmlPath = openFileDialog.FileName;
                this.txtFileName.Text = xmlPath;

                string xmlContent = File.ReadAllText(xmlPath);
                this.txtXMLContent.Text = xmlContent;

                MessageBox.Show("XML loaded successfully");

                // You can now:
                // ValidateXml(xmlContent, xsdPath);
                // DeserializeXml<YourClass>(xmlContent);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.txtEncryptedXML.Text = GetJsonContent();
        }
        private string GetJsonContent()
        {
            string base64 = Utils.EncodeToBase64(this.txtXMLContent.Text,this.txtKey.Text.ToString(),this.txtIV.Text.ToString());
            var request = new XmlRequest
            {
                FileName = Path.GetFileName(this.txtFileName.Text),
                Content = base64
            };

            return JsonSerializer.Serialize(request);
        }
        private async Task<string> CallAzureFunctionAsync(string json)
        {
            using HttpClient client = new HttpClient();

            string url = this.txtAzurePath.Text;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            this.txtAzureResponse.Text = await CallAzureFunctionAsync(txtEncryptedXML.Text);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            JsonResponse response = JsonSerializer.Deserialize<JsonResponse>(txtAzureResponse.Text, options);

            var decoded = Utils.DecodeFromBase64(response.Data, this.txtKey.Text.ToString(), this.txtIV.Text.ToString());

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(decoded);

            this.txtCustomerDataDecoded.Text =
                JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.txtXMLContent.Text = "";
            this.txtEncryptedXML.Text = "";
            this.txtAzureResponse.Text = "";
            this.txtCustomerDataDecoded.Text = "";
        }
    }
}
