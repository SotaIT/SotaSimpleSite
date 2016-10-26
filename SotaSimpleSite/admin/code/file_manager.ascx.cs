using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Web;
	using System.Data;
	using System.Drawing;
	using ICSharpCode.SharpZipLib.Zip;

	/// <summary>
	///	Файловый менеджер.
	/// </summary>
	public class SotaWebFileExplorer : System.Web.UI.UserControl
	{
		public FileInfo[] arFiles;


		public DirectoryInfo[] arDirs;

		private string _sCurPath = "";

		public string sCurPath
		{
			get { return _sCurPath; }
			set { _sCurPath = value.TrimEnd(Keys.UrlPathDelimiter[0]) + Keys.UrlPathDelimiter; }
		}

		private string curPath
		{
			get { return Request.MapPath(sCurPath).TrimEnd('\\'); }
		}

		public string _sCurUpPath = "";

		public string sCurUpPath
		{
			get { return _sCurUpPath; }
			set { _sCurUpPath = value.TrimEnd(Keys.UrlPathDelimiter[0]) + Keys.UrlPathDelimiter; }
		}

		public string _sRootPath = "";

		public string sRootPath
		{
			get { return _sRootPath; }
			set { _sRootPath = value.TrimEnd(Keys.UrlPathDelimiter[0]) + Keys.UrlPathDelimiter; }
		}

		private string rootPath
		{
			get { return Request.MapPath(sRootPath).TrimEnd('\\'); }
		}


		public string sFilter = "*";
		public string sPhisycalRoot = "";
		public string sVirtualRoot = "";
		public string sHtmlEditor = "";
		public string sError = "";
		private DataTable tbFT;
		private DataTable tbTemplates;
		private DataTable tbConf;
		public string[] arImgTypes;
		public string sOpenerField = "";
		public bool IsDialog = false;
		protected System.Web.UI.HtmlControls.HtmlInputFile idFile;

		private NumberFormatInfo nfi = new NumberFormatInfo();
		public bool HideAdminFolders = false;
		public int pageSize = 50;
		public int pageNumber = 0;

		protected override void OnLoad(EventArgs e)
		{
			#region init

			base.OnLoad(e);
			nfi.NumberGroupSeparator = " ";
			nfi.NumberGroupSizes = new int[] {3};
			nfi.NumberDecimalDigits = 0;
			tbFT = Config.GetConfigTable("explorer.config", "filetype");
			tbTemplates = Config.GetConfigTable("explorer.config", "filetemplate");
			sPhisycalRoot = Request.MapPath(Keys.ServerRoot);
			sVirtualRoot = ResolveUrl(Keys.ServerRoot);
			tbConf = Config.GetConfigTable("explorer.config", "common");
			arImgTypes = tbConf.Rows[0]["imgtypes"].ToString().Split(';');
			HideAdminFolders = tbConf.Rows[0]["ha"].ToString() != "0" || !Sota.Web.SimpleSite.Security.UserInfo.Current.IsInGroup(-3);

			#endregion

			#region read query string

			if (Request.QueryString["root"] == null
				|| Request.QueryString["root"] == string.Empty)
				sRootPath = sVirtualRoot;
			else
				sRootPath = Request.QueryString["root"];

			if (Request.QueryString["path"] == null)
				sCurPath = sRootPath;
			else
				sCurPath = Request.QueryString["path"];

			if (Request.QueryString["filter"] != null)
				sFilter = Request.QueryString["filter"];

			if (Request.QueryString["field"] != null)
				sOpenerField = Request.QueryString["field"];

			if (Request.QueryString["editor"] != null)
				sHtmlEditor = Request.QueryString["editor"];

			if (Request.QueryString["page"] != null)
				pageNumber = int.Parse(Request.QueryString["page"]);

			if (Request.QueryString["psize"] != null)
				pageSize = int.Parse(Request.QueryString["psize"]);

			#endregion

			string[] validFolders = tbConf.Rows[0]["folders"].ToString().Split(';');
			int n = validFolders.Length;
			bool bInValid = true;
			string rootForCheck = rootPath.ToLower() + "\\";
			for (int i = 0; i < n; i++)
			{
				if (rootForCheck.StartsWith(Request.MapPath(validFolders[i]).ToLower()))
				{
					bInValid = false;
					break;
				}

			}
			if (bInValid)
			{
				Response.End();
			}
			if (!Directory.Exists(rootForCheck))
			{
				Directory.CreateDirectory(rootForCheck);
			}

			IsDialog = sOpenerField.Length > 0;

			Select(curPath);

			if (Request.RequestType.ToLower() == "post")
			{
				PostBack();
			}
		}

		protected bool IsHidden(string name)
		{
			switch (name.ToLower())
			{
				case "admin":
				case "bin":
				case "config":
				case "data":
					return true;
			}
			return false;
		}

		private void Select(string path)
		{
			DirectoryInfo oDirInfo = new DirectoryInfo(path);
			if (HideAdminFolders)
			{
				if (IsHidden(oDirInfo.Name))
				{
					oDirInfo = new DirectoryInfo(Request.MapPath("~/"));
					sRootPath = sVirtualRoot;
					sCurPath = sRootPath;
				}
				DirectoryInfo tempDir = oDirInfo;
				while (tempDir.Parent != null)
				{
					tempDir = tempDir.Parent;
					if (IsHidden(tempDir.Name))
					{
						oDirInfo = new DirectoryInfo(Request.MapPath("~/"));
						sRootPath = sVirtualRoot;
						sCurPath = sRootPath;
						break;
					}
				}
			}
			if (oDirInfo.Parent == null)
			{
				sCurUpPath = sRootPath;
			}
			else
			{
				if (rootPath.ToLower().StartsWith(oDirInfo.Parent.FullName.ToLower()))
					sCurUpPath = sRootPath;
				else
					sCurUpPath = GetWebPath(oDirInfo.Parent.FullName);
			}
			arDirs = oDirInfo.GetDirectories();

			string[] arFilter = sFilter.Split(';');
			SortedList ar = new SortedList();
			foreach (string filter in arFilter)
			{
				arFiles = oDirInfo.GetFiles("*." + filter);
				int n = arFiles.Length;
				for (int i = 0; i < n; i++)
				{
					if (!HideAdminFolders || !(arFiles[i].Directory.Name.ToLower() == "data" && arFiles[i].Name.ToLower().StartsWith("admin.")))
					{
						if (!ar.ContainsKey(arFiles[i].Name))
						{
							ar[arFiles[i].Name] = arFiles[i];
						}
					}
				}
			}
			arFiles = new FileInfo[ar.Count];
			ar.Values.CopyTo(arFiles, 0);
		}

		private void PostBack()
		{
			string cPath = curPath + "\\";
			try
			{
				if (Request.Form["txtFolderName"] != null)
				{
					#region Создать папку

					if (Request.Form["txtFolderName"].Trim().Length > 0)
					{
						if(!Directory.Exists((cPath + Request.Form["txtFolderName"])))
						{
							Directory.CreateDirectory((cPath + Request.Form["txtFolderName"]));
						}
					}

					#endregion
				}
				else if (Request.Files.Count > 0)
				{
					if (Request.Form["hReplace"] == null)
					{
						#region Загрузка архива
						HttpPostedFile pf = Request.Files[0];
						if (pf.FileName.Length > 0)
						{
							using (ZipInputStream s = new ZipInputStream(pf.InputStream))
							{
								ZipEntry theEntry;
								while ((theEntry = s.GetNextEntry()) != null)
								{

									string directoryName = Request.MapPath(sCurPath + "/" + Path.GetDirectoryName(theEntry.Name));
									string fileName = Path.GetFileName(theEntry.Name);
									//Hashtable ht = new Hashtable();
									//ht["name"] = theEntry.Name;
									//ht["directoryName"] = directoryName;
									//ht["fileName"] = fileName;
									//Log.Write("entry", ht);
									//continue;
									// create directory
									
									if (directoryName.Length > 0)
									{
										Directory.CreateDirectory(directoryName);
									}

									if (fileName != String.Empty)
									{
										using (FileStream streamWriter = File.Create(directoryName + "\\" + fileName))
										{

											int size = 2048;
											byte[] data = new byte[2048];
											while (true)
											{
												size = s.Read(data, 0, data.Length);
												if (size > 0)
												{
													streamWriter.Write(data, 0, size);
												}
												else
												{
													break;
												}
											}
										}
									}
								}
							}
						}
						#endregion
					}
					else
					{
						#region Загрузка файла

						HttpPostedFile pf = Request.Files[0];
						if (pf.FileName.Length > 0)
						{
							//получение имени файла
							string[] af = pf.FileName.Split('\\');
							string file = af[af.Length - 1];

							//проверка типа файла
							bool wrongType = false;
							if (sFilter != "*")
							{
								wrongType = true;
								string[] arFilter = sFilter.ToLower().Split(';');
								string ext = this.GetExtention(file).ToLower();
								foreach (string fe in arFilter)
									if (ext == fe)
									{
										wrongType = false;
										break;
									}
							}
							if (!wrongType)
							{
								//создание уникального имени
								if (Request.Form["hReplace"] != "1")
								{
									int i = 0;
									while (File.Exists(cPath + file))
									{
										i++;
										file = i + file;
									}
								}
								//сохранение
								pf.SaveAs(cPath + file);
							}
							else
							{
								throw new Exception("Wrong file type.");
							}
						}

						#endregion
					}
				}
				else if (Request.Form["txtNewName"] != null
					&& Request.Form["txtOldName"] != null
					&& Request.Form["txtPreviewW"] != null
					&& Request.Form["txtPreviewH"] != null)
				{
					#region Сохранение с новым размером

					if (Request.Form["txtNewName"].Trim().Length > 0
						&& Request.Form["txtOldName"].Trim().Length > 0
						&& Request.Form["txtPreviewW"].Trim().Length > 0
						&& Request.Form["txtPreviewH"].Trim().Length > 0
						)
					{
						ImageFormat imageFormat = ImageFormat.Png;
						string sOldFile = Request.Form["txtOldName"].Trim();
						string sNewFile = Request.Form["txtNewName"].Trim();
						int nWidth = int.Parse(Request.Form["txtPreviewW"]);
						int nHeigth = int.Parse(Request.Form["txtPreviewH"]);
						string ext = GetExtention(sNewFile).ToLower();
						switch (ext)
						{
							case "gif":
								imageFormat = ImageFormat.Gif;
								break;
							case "jpg":
							case "jpeg":
								imageFormat = ImageFormat.Jpeg;
								break;
							case "png":
								imageFormat = ImageFormat.Png;
								break;
							case "bmp":
								imageFormat = ImageFormat.Bmp;
								break;
							default:
								sNewFile += ".png";
								break;
						}
						Bitmap bitmap			= null;
						Image image				= null;
						Graphics gr				= null;
						SmoothingMode sm		= (SmoothingMode)Enum.Parse(typeof(SmoothingMode), Request.Form["cmbSmoothingMode"], true);
						CompositingQuality cq	= (CompositingQuality)Enum.Parse(typeof(CompositingQuality), Request.Form["cmbCompositingQuality"], true);
						try
						{
							bitmap = new Bitmap(cPath + sOldFile);
							image = new Bitmap(nWidth, nHeigth);
							gr = Graphics.FromImage(image);
							gr.SmoothingMode = sm;
							gr.CompositingQuality = cq;
							gr.DrawImage(bitmap, 0, 0, nWidth, nHeigth);
							bitmap.Dispose();
							gr.Dispose();
							while (File.Exists(cPath + sNewFile))
							{
								sNewFile = "0" + sNewFile;
							}
							image.Save(cPath + sNewFile, imageFormat);
						}
						finally
						{
							if (gr != null)
								gr.Dispose();
							if (bitmap != null)
								bitmap.Dispose();
							if (image != null)
								image.Dispose();
						}
					}

					#endregion	
				}
				else if (Request.Form["txtFileName"] != null)
				{
					#region Создание файла

					string file = Request.Form["txtFileName"].Trim();
					if (Request.Form["cmbFileType"] == "*")
					{
						if(Request.Form["hReplace"]!="1")
						{
							int i=0;
							while (File.Exists(cPath + file))
							{
								i++;
								file = i + file;
							}
						}
						File.Create(cPath + file).Close();
					}
					else
					{
						string content = "";
						string cext = Request.Form["cmbFileType"].Split('-')[0];
						file = file + "." + cext;
						if(Request.Form["hReplace"]!="1")
						{
							int i=0;
							while (File.Exists(cPath + file))
							{
								i++;
								file = i + file;
							}
						}
						DataRow[] rows = tbTemplates.Select("id like '*;" + Request.Form["cmbFileType"] + ";*'");
						if (rows.Length > 0)
						{
							content = rows[0]["content"].ToString();

						}
						Encoding enc = Encoding.Default;
						if (IsXmlFile(file))
							enc = Encoding.UTF8;
						StreamWriter sw = null;
						try
						{
							sw = new StreamWriter(cPath + file, false, enc);
							sw.Write(content);
						}
						finally
						{
							if (sw != null)
								sw.Close();
						}
					}

					#endregion
				}
				else if (Request.Form["act"] == "delete")
				{
					#region Удаление файла

					string file = Request.MapPath(Request.Form["hFile"]).ToLower();
					if (File.Exists(file))
					{
						bool allowed = false;
						DataTable tb = Config.GetConfigTable("delete.config","folder");
						for(int i=0;i<tb.Rows.Count;i++)
						{
							string folder = Request.MapPath(tb.Rows[i]["name"].ToString()).ToLower();
							if(file.StartsWith(folder))
							{
								allowed = tb.Rows[i]["allow"].ToString()=="1";
								if(!allowed)
								{
									break;
								}
							}
						}
						if(allowed)
						{
							File.Delete(file);
						}
						else
						{
							throw new UnauthorizedAccessException("Удаление запрещено!");
						}
					}

					#endregion
				}
				else if (Request.Form["act"] == "deletefolder")
				{
					#region Удаление папки

					string folder = Request.MapPath(Request.Form["hFolder"]);
					if (Directory.Exists(folder))
					{
						if(Directory.GetFileSystemEntries(folder).Length>0)
						{
							Directory.Delete(folder);
						}
					}
					#endregion
				}
				Response.Redirect(Sota.Web.SimpleSite.Path.Full);
			}
			catch (Exception ex)
			{
				sError = ex.Message;
			}
		}

		public string GetWebPath(string physicalPath)
		{
			return 
				(sVirtualRoot 
				+ Keys.UrlPathDelimiter 
				+ physicalPath.Substring(sPhisycalRoot.Length, physicalPath.Length - sPhisycalRoot.Length) 
				+ Keys.UrlPathDelimiter
				).Replace("\\", Keys.UrlPathDelimiter)
				.Replace(Keys.UrlPathDelimiter + Keys.UrlPathDelimiter, Keys.UrlPathDelimiter)
				.TrimEnd(Keys.UrlPathDelimiter[0]);
		}

		private string GetExtention(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf(".") + 1);
		}

		public string GetFileImage(string fileName)
		{
			string ext = GetExtention(fileName);
			DataRow[] rows = tbFT.Select("extention like '*;" + ext + ";*'");
			if (rows.Length > 0)
			{
				return ResolveUrl(rows[0]["iconpath"].ToString());
			}
			else
			{
				return ResolveUrl(tbFT.Select("extention='*'")[0]["iconpath"].ToString());
			}
		}

		public string FormatBytes(long bytes)
		{
			if (bytes >= 1024)
			{
				if (bytes >= 1024 * 1024)
				{
					return string.Format("{0:#.##} Мб", bytes / 1024.0 / 1024);
				}
				return string.Format("{0:#.##} Кб", bytes / 1024.0);
			}
			return string.Format("{0} байт", bytes);
		}

		private bool IsXmlFile(string fileName)
		{
			string ext = GetExtention(fileName).ToLower();
			return ext == "xml" || ext == "config";
		}
	}
}