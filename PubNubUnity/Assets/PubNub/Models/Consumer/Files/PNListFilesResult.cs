using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNListFilesResult
    {
        public List<PNFileInfo> Data { get; set;}
        public int Count { get; set;}
        public string Next { get; set;}

    }
    public class PNFileInfo{
        public string Name { get; set;}
        public string ID { get; set;}
        public int Size { get; set;}
        public string Created { get; set;}
    }
}