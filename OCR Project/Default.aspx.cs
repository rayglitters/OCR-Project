using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Aspose.Pdf;
using System.Threading;
// </snippet_using>
using System.Drawing;
using BitMiracle.Docotic.Pdf;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Line = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.Line;
using Aspose.Pdf.Facades;

namespace OCR_Project
{
    public partial class _Default : System.Web.UI.Page
    {
        //Directory where all files are to be saved
        private const string dire = @"C:\Users\PC\Desktop\Trade Interchange\";

        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "5b2ba44cbfef4035999f2c89ead048b3";

        //Add the endpoint
        static string endpoint = "https://computervisionresourcearcus.cognitiveservices.azure.com/";

        protected void Page_Load(object sender, EventArgs e)
        {

            lblUploadResult.Text = "";
            //ANYTHING YOU WANT TO RUN ON PAGE LOAD CAN GO HERE, LIKE YOUR MAIN METHOD
            //IF YOU WANT METHODS TO BE TRIGGERED BY USER EVENTS RATHER THAN PAGE LOAD, YOU CAN ADD CONTROLS TO THE FRONT END AND HANDLE THEIR EVENTS E.G. BUTTON CLICK
           
        }

        protected void ExtractBtn_Click(object sender, EventArgs e)
        {
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
            string fileName = dire + FileUpLoad1.FileName;
            TextBox1.Text = fileName;
            //Image1.ImageUrl = fileName;

        }
        /// <summary>
        /// THIS METHOD WILL BE HIT WHEN THE USER CLICKS THE UPLOAD BUTTON
        /// </summary>
        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            if (FileUpLoad1.HasFile)
            {   
                //Save the uploaded file
                FileUpLoad1.SaveAs(dire +FileUpLoad1.FileName);
                lblUploadResult.Text = FileUpLoad1.FileName;
                //HERE YOU CAN PERFORM ACTIONS ON THE UPLOADED FILE
               string fileName = dire + FileUpLoad1.FileName;
                // get the file extension e.g., pdf or jpeg
                string extension = System.IO.Path.GetExtension(fileName);
                // Allow only files with pdf or jpeg extensions to be uploaded.
               
                if (extension == ".pdf")
                {
                    string result = CheckIfPdfContainsTextOrImages(fileName);
                    if (result == "T")
                    {
                        TextBox1.Text = ExtractTextFromPdf(fileName);
                        File.WriteAllText(dire + "a.txt", ExtractTextFromPdf(fileName));
                    }
                    else
                    {
                        var cTask = ReadFileURLTxt(client, fileName);
                        TextBox1.Text = cTask.Result;
                        //  Block of code to handle errors
                    }
                  
                    
                }
                
                else
                {
                    var cTask = ReadFileURLTxt(client, fileName);
                    TextBox1.Text = cTask.Result;
                    File.WriteAllText(dire + "a.txt", cTask.Result);
                }

                DisplayFileContents(FileUpLoad1.PostedFile);
            }
            else
            {
                lblUploadResult.Text = "No File Uploaded.";
            }
            

        }

        //PUT YOUR METHODS HERE E.G. AUTHENTICATE/PROCESS PDF ETC.
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }
        // </snippet_auth>
        /*
         * END - Authenticate
         */
        void DisplayFileContents(HttpPostedFile file)
        {
            System.IO.Stream myStream;
            Int32 fileLen;
            StringBuilder displayString = new StringBuilder();

            // Get the length of the file.
            fileLen = FileUpLoad1.PostedFile.ContentLength;

            // Display the length of the file in a label.
            LengthLabel.Text = "The length of the file is " +
                               fileLen.ToString() + " bytes.";

            // Create a byte array to hold the contents of the file.
            Byte[] Input = new Byte[fileLen];

            // Initialize the stream to read the uploaded file.
            myStream = FileUpLoad1.FileContent;

            // Read the file into the byte array.
            myStream.Read(Input, 0, fileLen);

            // Copy the byte array to a string.
            for (int loop1 = 0; loop1 < fileLen; loop1++)
            {
                displayString.Append(Input[loop1].ToString());
            }


        }

        public static async Task<string> ReadFileURLTxt(ComputerVisionClient client, string FileUrl)
        {
            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(File.OpenRead(FileUrl)).ConfigureAwait(false);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);
            // </snippet_readfileurl_1>

            // <snippet_readfileurl_2>
            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId)).ConfigureAwait(false);
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            // </snippet_readfileurl_2>
            // Display the found text.
            StringBuilder text = new StringBuilder(); 
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    text.Append(line.Text +Environment.NewLine);
                }
            }
            return text.ToString();

        }
        // </snippet_readfileurl_3>

        /*
         * END - READ FILE - URL
         */


        //This code works but for only one page
        string  ReadFilePDF(string pdfFile)
        {
            using (var pdf = new BitMiracle.Docotic.Pdf.PdfDocument(pdfFile))
            {
                string documentText = pdf.GetText();
                return documentText;
            }
            
        }

        //This code works for more than one page
        public static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }
        //Check if the PDF has image or not
        public static string CheckIfPdfContainsTextOrImages(string path)
        {
            // Instantiate a memoryStream object to hold the extracted text from Document
            MemoryStream ms = new MemoryStream();
            // Instantiate PdfExtractor object
            PdfExtractor extractor = new PdfExtractor();

            // Bind the input PDF document to extractor
            extractor.BindPdf(path);
            // Extract text from the input PDF document
            extractor.ExtractText();
            // Save the extracted text to a text file
            extractor.GetText(ms);
            // Check if the MemoryStream length is greater than or equal to 1

            bool containsText = ms.Length >= 1;

            // Extract images from the input PDF document
            extractor.ExtractImage();

            // Calling HasNextImage method in while loop. When images will finish, loop will exit
            bool containsImage = extractor.HasNextImage();

            // Now find out whether this PDF is text only or image only

            if (containsText && !containsImage)
            {
                string t = "T";
                return t;
            }
            else if (!containsText && containsImage)
            {
                string i = "I";
                return i;
            }
            else if (containsText && containsImage)
            {
                string it = "IT";
                return it;
            }
            else
            {
                string n = "N";
                return n;
            }
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}