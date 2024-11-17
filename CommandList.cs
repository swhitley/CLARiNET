using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CLARiNET
{
    public static class Commands {

        public const string COMMAND_LIST = "CLAR_UPLOAD\nCLAR_DOWNLOAD\nDRIVE_UPLOAD\nDRIVE_TRASH\nPHOTO_DOWNLOAD\nPHOTO_UPLOAD\nDOCUMENT_UPLOAD\nCANDIDATE_ATTACHMENT_UPLOAD";

    }

    public enum Command
    {
        CLAR_UPLOAD,
        CLAR_DOWNLOAD,
        DRIVE_UPLOAD,
        DRIVE_TRASH,
        PHOTO_DOWNLOAD,
        PHOTO_UPLOAD,
        DOCUMENT_UPLOAD,
        CANDIDATE_ATTACHMENT_UPLOAD
    }
}
