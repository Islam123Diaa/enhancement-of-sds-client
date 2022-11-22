using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SC3_Alarm_Application_Server
{
    public class Auditing
    {
        private static System.Reflection.MethodBase myClassMethod = null;
        private static StreamWriter streamWriterAduit = null;
        private static StreamWriter streamWriterErrorAndMessage = null;
        //private static string AuditfilePath = "C:\\Program Files (x86)\\TETRA Voice Recorder v1.2\\TETRA Voice Recoder V1.2\\Files\\Auditing\\Audit";
        //private static string ErrorAndMessagefilePath = "C:\\Program Files (x86)\\TETRA Voice Recorder v1.2\\TETRA Voice Recoder V1.2\\Files\\Auditing\\ErrorAndMessage";
        private static string AuditfilePath = AppDomain.CurrentDomain.BaseDirectory + @"Files\Auditing\Audit";
        private static string ErrorAndMessagefilePath = AppDomain.CurrentDomain.BaseDirectory + @"Files\Auditing\ErrorAndMessage";
        private static string fileName = null;
        private static bool errorOrMessage = false;

        private static void Audit(string className, string methodName, string comment)
        {
            string output = "Class = " + className + " , Method = " + methodName + " , Action : \"" + comment + "\"";
            Console.WriteLine(output);
            WritingInAuditFile(output);
        }
        public static void Audit(string comment)
        {
            myClassMethod = new System.Diagnostics.StackFrame(1).GetMethod();
            string output = "Class = " + myClassMethod.ReflectedType.Name + " , Method = " + myClassMethod.Name + " , Action : \"" + comment + "\"";
            Console.WriteLine(output);
            WritingInAuditFile(output);
        }
        private static void Error(string className, string methodName, string error)
        {
            string output = "Class = " + className + " , Method = " + methodName + " , Error = " + error;
            Console.WriteLine(output);
            WritingInAuditFile(output);
            WritingInErrorAndMessageFile(output);
        }
        public static void Error(string error)
        {
            myClassMethod = new System.Diagnostics.StackFrame(1).GetMethod();
            string output = "\r\n Class = " + myClassMethod.ReflectedType.Name + " , Method = " + myClassMethod.Name + " , Error = " + error;
            Console.WriteLine(output);
            WritingInAuditFile(output);
            WritingInErrorAndMessageFile(output);
        }
        private static DialogResult MessageBox(string className, string methodName, string message, MessageBoxButtons messageButtons, MessageBoxIcon messageIcon)
        {
            string output = "Class = " + className + ", Method = " + methodName + " , Message = \"" + message + " \"";
            Console.WriteLine(output);
            WritingInAuditFile(output);
            WritingInErrorAndMessageFile(output);

            return System.Windows.Forms.MessageBox.Show(message, "TETRA Voice Recorder", messageButtons, messageIcon);
        }
        public static DialogResult MessageBox(string message, MessageBoxButtons messageButtons, MessageBoxIcon messageIcon)
        {
            MessageBoxBase(message);
            return System.Windows.Forms.MessageBox.Show(message, "TETRA Voice Recorder", messageButtons, messageIcon);
        }

        public delegate DialogResult MessageBoxDelegate(IWin32Window owner,
            string message, MessageBoxButtons messageButtons, MessageBoxIcon messageIcon);

        public static DialogResult MessageBox(IWin32Window owner,
            string message, MessageBoxButtons messageButtons, MessageBoxIcon messageIcon)
        {
            if (((Form)owner).InvokeRequired)
            {
                MessageBoxDelegate myDelegate = new MessageBoxDelegate(MessageBox);
                ((Form)owner).Invoke(myDelegate,
                    new object[] { owner, message, messageButtons, messageIcon });
                return new DialogResult();
            }
            else
            {
                MessageBoxBase(message);
                return System.Windows.Forms.MessageBox.Show(owner, message, "TETRA Voice Recorder", messageButtons, messageIcon);
            }
        }
        private static void MessageBoxBase(string message)
        {
            myClassMethod = new System.Diagnostics.StackFrame(2).GetMethod();
            string output = "Class = " + myClassMethod.ReflectedType.Name +
                " , Method = " + myClassMethod.Name + " , Message =  \"" +
                message + " \"";
            Console.WriteLine(output);
            WritingInAuditFile(output);
            //WritingInErrorAndMessageFile(output);
        }

        public static void ClearAuditFile()
        {
            streamWriterAduit.Close();
            streamWriterAduit = new
                StreamWriter(new FileStream(AuditfilePath + fileName, FileMode.Truncate));
        }
        public static void InitializeCreatingAuditAndErrorFile()
        {
            fileName = "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + "_" +
                          DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";
            streamWriterAduit = new StreamWriter(new FileStream(AuditfilePath + fileName, FileMode.CreateNew));

            streamWriterErrorAndMessage = new StreamWriter(new FileStream(ErrorAndMessagefilePath + fileName, FileMode.CreateNew));
        }
        public static void ClosingAuditAndErrorFile()
        {
            streamWriterAduit.Close();
            streamWriterErrorAndMessage.Close();
            CheckErrorFile();

            streamWriterAduit = null;
            streamWriterErrorAndMessage = null;
            AuditfilePath = null;
            ErrorAndMessagefilePath = null;
            fileName = null;

        }
        private static void WritingInAuditFile(string audit)
        {
            streamWriterAduit.WriteLine(audit);
            streamWriterAduit.Flush();
        }
        private static void WritingInErrorAndMessageFile(string errorAndMessage)
        {
            streamWriterErrorAndMessage.WriteLine(errorAndMessage);
            streamWriterErrorAndMessage.Flush();
            errorOrMessage = true;
        }
        private static void CheckErrorFile()
        {
            //====================================================================
            // deleting ErrorAndMessage file if no error or message write in it 
            //====================================================================
            if (!errorOrMessage)
                File.Delete(ErrorAndMessagefilePath + fileName);
        }
    }
}
