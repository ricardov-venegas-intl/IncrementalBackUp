using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IncrementalBackup
{
    interface IFileHasher
    {
        string CalculateHash(FileStream fileStream);
    }
}
