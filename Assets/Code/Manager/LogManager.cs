using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Manager
{
    public enum LogType
    {
        Debug,
        Error,
        Warning,
        Item,
        Battle,
    }

    public struct LogInfo
    {
        LogType type;
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

        /// <summary>
        /// 로그 파일에 남길 로그 데이터를 큐에 넣는 메소드
        /// </summary>
        /// <param name="logInfo"></param>
        public void Enqueue(LogInfo logInfo)
        {
            lock(lockObject)
            {
                logQueue.Enqueue(logInfo);
            }
        }

        /// <summary>
        /// 저장된 로그 큐를 로그 파일에 작성하는 메소드
        /// </summary>
        private void Run()
        {
            Thread logThread = new Thread(WriteLogAtFile);
            logThread.Start();
            logThread.Join();
        }

        /// <summary>
        /// 로그를 파일에 작성하는 메소드
        /// </summary>
        private void WriteLogAtFile()
        {
            lock(lockObject)
            {
                for(int i = 0; i < logQueue.Count; i++)
                {
                    /// TODO: 카테고리에 따라 
                }
            }
        }

        /// <summary>
        /// 콜솔에 로그를 출력하는 메소드
        /// </summary>
        /// <param name="title">로그 제목</param>
        /// <param name="msg">로그 상세 메시지</param>
        public static void ConsoleLog(string title, string msg)
        {
            ConsoleDebugLog(title, msg);
        }

        /// <summary>
        /// 콜솔에 로그를 출력하는 메소드
        /// </summary>
        /// <param name="title">로그 제목</param>
        /// <param name="msg">로그 상세 메시지</param>
        /// <param name="msg"></param>
        public static void ConsoleLog(LogType catrgory, string title, string msg)
        {
            switch(catrgory)
            {
                case LogType.Debug:
                    ConsoleDebugLog(title, msg);
                    break;

                case LogType.Error:
                    ConsoleErrorLog(title, msg);
                    break;

                case LogType.Warning:
                    ConsoleWarningLog(title, msg);
                    break;
            }
        }

        /// <summary>
        /// 콘솔에 디버그용 로그를 출력하는 메소드
        /// </summary>
        /// <param name="title">로그 제목</param>
        /// <param name="msg">로그 상세 메시지</param>
        public static void ConsoleDebugLog(string title, string msg)
        {
            Debug.Log($"<color=green><b>[{title}]</b></color> {msg}");
        }

        /// <summary>
        /// 콘솔에 에러용 로그를 출력하는 메소드
        /// </summary>
        /// <param name="title">로그 제목</param>
        /// <param name="msg">로그 상세 메시지</param>
        public static void ConsoleErrorLog(string title, string msg)
        {
            Debug.LogError($"<color=yellow><b>[{title}]</b></color> {msg}");
        }

        /// <summary>
        /// 콘솔에 위험용 로그를 출력하는 메소드
        /// </summary>
        /// <param name="title">로그 제목</param>
        /// <param name="msg">로그 상세 메시지</param>
        public static void ConsoleWarningLog(string title, string msg)
        {
            Debug.LogWarning($"<color=red><b>[{title}]</b></color> {msg}");
        }
    }
}