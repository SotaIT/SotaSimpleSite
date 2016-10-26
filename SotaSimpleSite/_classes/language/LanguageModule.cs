//using System;
//using System.Web;
//
//namespace Sota.Web.SimpleSite
//{
//    public class LanguageModule : IHttpModule
//    {
//        public void Dispose()
//        {
//        }
//
//        public void Init(HttpApplication context)
//        {
//            context.BeginRequest += new EventHandler(context_BeginRequest);
//        }
//
//        void context_BeginRequest(object sender, EventArgs e)
//        {
//			LanguageInfo.Init();
//		}
//    }
//}
