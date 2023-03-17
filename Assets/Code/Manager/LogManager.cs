using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Manager
{
    public enum LogCatrgory
    {
        Error,
        Warning,
        Item,
        Battle,
    }

    public struct LogInfo
    {
        LogCatrgory catrgory;
        string fileName;
        Queue<string> context;
    }

    /// <summary>
    /// 게임에서 발생하는 로그 및 로그파일 관리자
    /// </summary>
    /// <remarks>
    /// 게임 데모 완성을 위해 잠시 개발 보류
    /// </remarks>
    public class LogManager : MonoSingleton<LogManager>
    {
        private Queue<LogInfo> logQueue = new Queue<LogInfo>();
        private Queue<LogInfo> tmpQueue = new Queue<LogInfo>();
        private object lockObject;

        public void Enqueue(LogInfo logInfo)
        {
            lock(lockObject)
            {
                logQueue.Enqueue(logInfo);
            }
        }

        private void Run()
        {
            Thread logThread = new Thread(Log);
            logThread.Start();
            logThread.Join();
        }

        private void Log()
        {
            lock(lockObject)
            {
                for(int i = 0; i < logQueue.Count; i++)
                {
                    /// TODO: 카테고리에 따라 
                }
            }
        }
    }
}