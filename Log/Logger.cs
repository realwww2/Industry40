﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Text;

namespace I4.Log
{
    public class Logger
    {
        private string _basePath;
        private string _errorFileStr;
        private string _debugFileStr;
        public Logger(string basePath,string errorFileStr,string debugFileStr)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            _basePath = basePath;
            _errorFileStr = errorFileStr;
            _debugFileStr = debugFileStr;
        }
        private string ErrorFileName
        {
            get { return GetFileByKey(_basePath, _errorFileStr); }
        }
        private string DebugFileName
        {
            get { return GetFileByKey(_basePath, _debugFileStr); }
        }
        public void LogError(string str)
        {
            str = string.Format("\n{0}\t{1}", DateTime.Now.ToString("dd-MM-yyyy hh mm ss"), str);
            File.AppendAllText(ErrorFileName, str);
        }
        public void LogError(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            CreateExceptionString(sb, e.InnerException, "    ");
            LogError(sb.ToString());
        }

        public void LogDebug(string str)
        {
            File.AppendAllText(DebugFileName, "\n" + str);
        }

        private string GetFileByKey(string basePath, string key)
        {
            key = key.Replace("[Date]", DateTime.Now.Date.ToString("dd-MM-yyyy"));
            key = key.Replace("[Time]", DateTime.Now.ToString("dd-MM-yyyy hh mm ss"));
            return string.Format("{0}\\{1}", basePath, key);
        }

        private void CreateExceptionString(StringBuilder sb, Exception e, string indent)
        {
            sb.AppendFormat("\n{0}", indent); sb.AppendFormat("inner exception {0}",e.GetType().FullName);            
            sb.AppendFormat("\n{0}Message: {1}", indent, e.Message);
            sb.AppendFormat("\n{0}Source: {1}", indent, e.Source);
            sb.AppendFormat("\n{0}Stacktrace: {1}", indent, e.StackTrace);

            if (e.InnerException != null)
            {
                CreateExceptionString(sb, e.InnerException, indent + indent);
            }
        }
    }
}