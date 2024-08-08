using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHRepoLetterStats.Common.Configuration;
public class Configuration
{
    public string[] SubExtensionsToIgnore { get; set; } = new string[] { ".spec" };
}
