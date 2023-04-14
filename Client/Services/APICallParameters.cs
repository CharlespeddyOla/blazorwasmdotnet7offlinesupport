namespace WebAppAcademics.Client.Services
{
    public static class APICallParameters
    {
        static int _id;

        public static int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        static string _urlget;

        public static string RequestUriGet
        {
            get
            {
                return _urlget;
            }
            set
            {
                _urlget = value;
            }
        }

        static string _urlgetby;

        public static string RequestUriGetBy
        {
            get
            {
                return _urlgetby;
            }
            set
            {
                _urlgetby = value;
            }
        }

        static string _urlsave;

        public static string RequestUriSave
        {
            get
            {
                return _urlsave;
            }
            set
            {
                _urlsave = value;
            }
        }

        static string _urlupdate;

        public static string RequestUriUpdate
        {
            get
            {
                return _urlupdate;
            }
            set
            {
                _urlupdate = value;
            }
        }

        static string _urldelete;

        public static string RequestUriDelete
        {
            get
            {
                return _urldelete;
            }
            set
            {
                _urldelete = value;
            }
        }

        static bool _isauth = false;

        public static bool IsAuth
        {
            get
            {
                return _isauth;
            }
            set
            {
                _isauth = value;
            }
        }
    }
}
