using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace ComputerVisionDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label7.Visible = false;
        }

        public async void GetResultsFromApi(string key, string url, string imagePath)
        {
            label7.Visible = true;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(imagePath);
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
            }
            string contentString = await response.Content.ReadAsStringAsync();
            richTextBox1.Text = Newtonsoft.Json.Linq.JToken.Parse(contentString).ToString();
            label7.Visible = false;
        }
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK) textBox2.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            string imagePath = textBox2.Text;
            string endpoint = textBox3.Text;
            string key = textBox1.Text;
            string serviceType = comboBox1.SelectedItem.ToString();
            string url = endpoint + "/vision/v2.0/";
            switch (serviceType)
            {
                case "Analyse":
                    url += "analyze";
                    break;
                case "Describe":
                    url += "describe";                    
                    break;
                case "Tag":
                    url += "tag";
                    break;
                case "Detect Objects":
                    url += "detect";
                    break;
                case "List Models":
                    url += "models";
                    break;
                case "Recognize Text":
                    url += "ocr?detectOrientation=true";
                    break;
            }
            GetResultsFromApi(key, url, imagePath);
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox1.Text = "";
            comboBox1.Text = "";
            richTextBox1.Text = "";
        }
    }
}
