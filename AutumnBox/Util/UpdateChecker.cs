﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;

namespace AutumnBox.Util
{
    /// <summary>
    /// 更新检测器,使用Check方法检测完成后会执行UpdateCheckFinish事件
    /// </summary>
    internal class UpdateChecker
    {
        public delegate void UpdateCheckFinishHandler(bool haveUpdate, VersionInfo updateVersionInfo);
        public event UpdateCheckFinishHandler UpdateCheckFinish;
        public void Check()
        {
            if (UpdateCheckFinish == null)
            {
                throw new NotSetEventHandlerException("没有设置事件!这还检测个皮皮虾的更新啊!!!");
            }
            Thread UpdateCheckThread = new Thread(_Check);
            UpdateCheckThread.Name = "Update Check Thread";
            UpdateCheckThread.Start();
        }
        private void _Check()
        {
            VersionInfo newVersionInfo = GetUpdateInfo();
            if (StaticData.nowVersion.build < newVersionInfo.build&&Config.skipBuild != newVersionInfo.build)
            {
                UpdateCheckFinish(true, newVersionInfo);
            }
            else
            {
                UpdateCheckFinish(false, newVersionInfo);
            }
        }
        private static VersionInfo GetUpdateInfo()
        {
            JObject j;
#if TEST_LOCAL_API
            j = JObject.Parse(File.ReadAllText("../Api/update.json"));
#else
            j = JObject.Parse(Tools.GetHtmlCode("https://raw.githubusercontent.com/zsh2401/AutumnBox/master/Api/update.json"));
#endif
            return new VersionInfo
            {
                version = j["Version"].ToString(),
                build = int.Parse(j["Build"].ToString()),
                content = j["UpdateContent"].ToString(),
                time = new DateTime(
                    year: int.Parse(j["Date"][0].ToString()),
                    month: int.Parse(j["Date"][1].ToString()),
                    day: int.Parse(j["Date"][2].ToString())),
                baiduPanDownloadUrl = j["BaiduPan"].ToString(),
                githubReleaseDownloadUrl = j["GithubRelease"].ToString()
            };

        }
    }
}

