using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Search_engine
{
    class DataSource
    {
        private enum RequiredFiles {Documents, Keywords};

        public List<Document> Documents { get; private set; }
        public List<string> Keywords { get; private set; }
        public bool IsReady
        {
            get
            {
                foreach (var v in _loaded)
                {
                    if (v == false)
                        return false;
                }
                return true;
            }
        }
        private bool[] _loaded;

        public DataSource()
        {
            Documents = new List<Document>();
            Keywords = new List<string>();
            _loaded = new bool[2];
        }

        public List<Document> LoadDocuments()
        {
            string filename = ChooseFile();
            string line;
            string content = string.Empty;
            string title = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                        {
                            Documents.Add(new Document { Title = title, Content = content }) ;
                            title = string.Empty;
                            content = string.Empty;
                        } else
                        {
                            if (title == string.Empty)
                                title = line;
                            else
                                content += line;
                        }
                    }  
                }
                _loaded[(int)RequiredFiles.Documents] = true;
                return Documents;
            }
            catch (Exception e)
            {
                //throw e;
            }
            return Documents;
        }

        public List<string> LoadKeywords()
        {
            string filename = ChooseFile();
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    while ((line = sr.ReadLine()) != null)
                        Keywords.Add(line);
                }
                _loaded[(int)RequiredFiles.Keywords] = true;
                
            }
            catch (Exception e)
            {
                //throw e;
            }
            return Keywords;
        }

        private string ChooseFile()
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.ShowDialog();

            return dialog.FileName;
        }
    }
}
