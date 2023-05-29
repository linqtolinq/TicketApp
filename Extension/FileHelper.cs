namespace TicketAppApi.Extension
{
    public static class FileHelper
    {
        public static byte[] StreamToBytes(this Stream stream)

        {

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;

        }

        /// 将 byte[] 转成 Stream

        public static Stream BytesToStream(this byte[] bytes)

        {

            Stream stream = new MemoryStream(bytes);

            return stream;

        }
    }
}
