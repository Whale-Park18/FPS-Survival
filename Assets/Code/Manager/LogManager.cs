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
    /// ���ӿ��� �߻��ϴ� �α� �� �α����� ������
    /// </summary>
    /// <remarks>
    /// ���� ���� �ϼ��� ���� ��� ���� ����
    /// </remarks>
    public class LogManager : MonoSingleton<LogManager>
    {
        private Queue<LogInfo> logQueue = new Queue<LogInfo>();
        private Queue<LogInfo> tmpQueue = new Queue<LogInfo>();
        private object lockObject;

        /// <summary>
        /// �α� ���Ͽ� ���� �α� �����͸� ť�� �ִ� �޼ҵ�
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
        /// ����� �α� ť�� �α� ���Ͽ� �ۼ��ϴ� �޼ҵ�
        /// </summary>
        private void Run()
        {
            Thread logThread = new Thread(WriteLogAtFile);
            logThread.Start();
            logThread.Join();
        }

        /// <summary>
        /// �α׸� ���Ͽ� �ۼ��ϴ� �޼ҵ�
        /// </summary>
        private void WriteLogAtFile()
        {
            lock(lockObject)
            {
                for(int i = 0; i < logQueue.Count; i++)
                {
                    /// TODO: ī�װ��� ���� 
                }
            }
        }

        /// <summary>
        /// �ݼֿ� �α׸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="title">�α� ����</param>
        /// <param name="msg">�α� �� �޽���</param>
        public static void ConsoleLog(string title, string msg)
        {
            ConsoleDebugLog(title, msg);
        }

        /// <summary>
        /// �ݼֿ� �α׸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="title">�α� ����</param>
        /// <param name="msg">�α� �� �޽���</param>
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
        /// �ֿܼ� ����׿� �α׸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="title">�α� ����</param>
        /// <param name="msg">�α� �� �޽���</param>
        public static void ConsoleDebugLog(string title, string msg)
        {
            Debug.Log($"<color=green><b>[{title}]</b></color> {msg}");
        }

        /// <summary>
        /// �ֿܼ� ������ �α׸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="title">�α� ����</param>
        /// <param name="msg">�α� �� �޽���</param>
        public static void ConsoleErrorLog(string title, string msg)
        {
            Debug.LogError($"<color=yellow><b>[{title}]</b></color> {msg}");
        }

        /// <summary>
        /// �ֿܼ� ����� �α׸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="title">�α� ����</param>
        /// <param name="msg">�α� �� �޽���</param>
        public static void ConsoleWarningLog(string title, string msg)
        {
            Debug.LogWarning($"<color=red><b>[{title}]</b></color> {msg}");
        }
    }
}