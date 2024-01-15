using Newtonsoft.Json;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{

    public abstract class MessageBase : IDisposable
    {
        public string SID { get; set; }
        public string EQName { get; set; }

        [JsonProperty("System Bytes")]
        public int SystemBytes { get; set; }

        [NonSerialized]
        public string OriJsonString;
        private bool disposedValue;

        internal DateTime createdTime { get; private set; } = DateTime.MinValue;
        internal bool used = false;
        public Dictionary<string, MessageHeader> Header { get; set; }
        public MessageBase()
        {
            createdTime = DateTime.Now;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                Header = null;
                OriJsonString = null;
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~MessageBase()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
