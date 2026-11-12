namespace Test_Azure_App
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtFileName = new TextBox();
            label1 = new Label();
            button1 = new Button();
            txtXMLContent = new TextBox();
            label2 = new Label();
            txtEncryptedXML = new TextBox();
            txtAzureResponse = new TextBox();
            label3 = new Label();
            label4 = new Label();
            btnEncryptXML = new Button();
            button3 = new Button();
            button2 = new Button();
            txtCustomerDataDecoded = new TextBox();
            label5 = new Label();
            txtAzurePath = new TextBox();
            button4 = new Button();
            label6 = new Label();
            label7 = new Label();
            txtKey = new TextBox();
            txtIV = new TextBox();
            SuspendLayout();
            // 
            // txtFileName
            // 
            txtFileName.Location = new Point(229, 13);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(786, 31);
            txtFileName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 16);
            label1.Name = "label1";
            label1.Size = new Size(38, 25);
            label1.TabIndex = 1;
            label1.Text = "File";
            // 
            // button1
            // 
            button1.Location = new Point(1021, 11);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 2;
            button1.Text = "Load XML";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // txtXMLContent
            // 
            txtXMLContent.Location = new Point(37, 287);
            txtXMLContent.Multiline = true;
            txtXMLContent.Name = "txtXMLContent";
            txtXMLContent.ScrollBars = ScrollBars.Both;
            txtXMLContent.Size = new Size(395, 433);
            txtXMLContent.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 257);
            label2.Name = "label2";
            label2.Size = new Size(146, 25);
            label2.TabIndex = 4;
            label2.Text = "XML File Content";
            // 
            // txtEncryptedXML
            // 
            txtEncryptedXML.Location = new Point(531, 287);
            txtEncryptedXML.Multiline = true;
            txtEncryptedXML.Name = "txtEncryptedXML";
            txtEncryptedXML.ScrollBars = ScrollBars.Both;
            txtEncryptedXML.Size = new Size(395, 433);
            txtEncryptedXML.TabIndex = 5;
            // 
            // txtAzureResponse
            // 
            txtAzureResponse.Location = new Point(1038, 289);
            txtAzureResponse.Multiline = true;
            txtAzureResponse.Name = "txtAzureResponse";
            txtAzureResponse.ScrollBars = ScrollBars.Both;
            txtAzureResponse.Size = new Size(359, 433);
            txtAzureResponse.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(531, 257);
            label3.Name = "label3";
            label3.Size = new Size(131, 25);
            label3.TabIndex = 7;
            label3.Text = "Encrypted XML";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1038, 259);
            label4.Name = "label4";
            label4.Size = new Size(471, 25);
            label4.TabIndex = 8;
            label4.Text = "Azure Function Response (Json) - Customer Data Encoded";
            // 
            // btnEncryptXML
            // 
            btnEncryptXML.Location = new Point(438, 471);
            btnEncryptXML.Name = "btnEncryptXML";
            btnEncryptXML.Size = new Size(87, 66);
            btnEncryptXML.TabIndex = 9;
            btnEncryptXML.Text = "Encrypt XML";
            btnEncryptXML.UseVisualStyleBackColor = true;
            btnEncryptXML.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(932, 471);
            button3.Name = "button3";
            button3.Size = new Size(101, 66);
            button3.TabIndex = 10;
            button3.Text = "Call Azure Function";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(1403, 473);
            button2.Name = "button2";
            button2.Size = new Size(116, 90);
            button2.TabIndex = 11;
            button2.Text = "Decrypt Customer Data";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // txtCustomerDataDecoded
            // 
            txtCustomerDataDecoded.Location = new Point(1542, 289);
            txtCustomerDataDecoded.Multiline = true;
            txtCustomerDataDecoded.Name = "txtCustomerDataDecoded";
            txtCustomerDataDecoded.ScrollBars = ScrollBars.Both;
            txtCustomerDataDecoded.Size = new Size(359, 433);
            txtCustomerDataDecoded.TabIndex = 12;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(31, 61);
            label5.Name = "label5";
            label5.Size = new Size(169, 25);
            label5.TabIndex = 13;
            label5.Text = "Azure Function Path";
            // 
            // txtAzurePath
            // 
            txtAzurePath.Location = new Point(229, 61);
            txtAzurePath.Name = "txtAzurePath";
            txtAzurePath.Size = new Size(786, 31);
            txtAzurePath.TabIndex = 14;
            txtAzurePath.Text = "http://localhost:7035/api/ProcessData";
            // 
            // button4
            // 
            button4.Location = new Point(1151, 12);
            button4.Name = "button4";
            button4.Size = new Size(112, 34);
            button4.TabIndex = 15;
            button4.Text = "Clear Result";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(29, 121);
            label6.Name = "label6";
            label6.Size = new Size(40, 25);
            label6.TabIndex = 16;
            label6.Text = "Key";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(31, 166);
            label7.Name = "label7";
            label7.Size = new Size(28, 25);
            label7.TabIndex = 17;
            label7.Text = "IV";
            // 
            // txtKey
            // 
            txtKey.Location = new Point(229, 121);
            txtKey.Name = "txtKey";
            txtKey.Size = new Size(786, 31);
            txtKey.TabIndex = 18;
            txtKey.Text = "Qm9V8m3k2xP9rT6nW4sY0A==";
            // 
            // txtIV
            // 
            txtIV.Location = new Point(229, 166);
            txtIV.Name = "txtIV";
            txtIV.Size = new Size(786, 31);
            txtIV.TabIndex = 19;
            txtIV.Text = "t8H2kL9vQm3xZp6sY1dF3g==";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1924, 814);
            Controls.Add(txtIV);
            Controls.Add(txtKey);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(button4);
            Controls.Add(txtAzurePath);
            Controls.Add(label5);
            Controls.Add(txtCustomerDataDecoded);
            Controls.Add(button2);
            Controls.Add(button3);
            Controls.Add(btnEncryptXML);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtAzureResponse);
            Controls.Add(txtEncryptedXML);
            Controls.Add(label2);
            Controls.Add(txtXMLContent);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(txtFileName);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFileName;
        private Label label1;
        private Button button1;
        private TextBox txtXMLContent;
        private Label label2;
        private TextBox txtEncryptedXML;
        private TextBox txtAzureResponse;
        private Label label3;
        private Label label4;
        private Button btnEncryptXML;
        private Button button3;
        private Button button2;
        private TextBox txtCustomerDataDecoded;
        private Label label5;
        private TextBox txtAzurePath;
        private Button button4;
        private Label label6;
        private Label label7;
        private TextBox txtKey;
        private TextBox txtIV;
    }
}
