using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Security;

namespace ModulusFE.Security
{
    ///<summary>
    /// Exception thrown in case there is something wrong with license file
    ///</summary>
    public class SecurityException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mesage"></param>
        public SecurityException(string mesage)
            : base(mesage)
        {

        }
    }

    internal class LicenseChecker
    {
        private bool _loaded;
        private class LicenseInfo
        {
            public string ClientName;
            public int Type;
            public DateTime Timestamp;
            public int ValidDays;
        }

        private LicenseInfo _licenseInfo;
        private readonly AesSecurity _security = new AesSecurity();

        private LicenseChecker() { }

        private static LicenseChecker _instance;
        public static LicenseChecker I
        {
            get
            {
                if (_instance == null)
                    _instance = new LicenseChecker();
                return _instance;
            }
        }

        public string CheckLicense(string filename)
        {
            if (!_loaded)
                LoadLicenseFile(filename);

            string res = _licenseInfo.ClientName + " Expires at: ";
            bool valid = false;
            if (_licenseInfo.Type == 0)
            {
                valid = DateTime.Now <= _licenseInfo.Timestamp.AddDays(_licenseInfo.ValidDays);
                res += _licenseInfo.Timestamp.AddDays(_licenseInfo.ValidDays).ToString();
            }
            if (_licenseInfo.Type == 1)
            {
                valid = DateTime.Now <= _licenseInfo.Timestamp;
                res += _licenseInfo.Timestamp.ToString();
            }
            if (!valid)
                throw new SecurityException("License expired. Please contact sales@modulusfe.com for a new license file.");

            return res;
        }

        private void LoadLicenseFile(string filename)
        {
            try
            {
                using (FileStream fs = File.OpenRead(filename))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string[] licInfo = _security.Decrypt(sr.ReadToEnd()).Split(new[] { '|' });

                        _licenseInfo = new LicenseInfo
                                         {
                                             ClientName = licInfo[0],
                                             Type = Convert.ToInt16(licInfo[1], CultureInfo.InvariantCulture),
                                             Timestamp = Convert.ToDateTime(licInfo[2], CultureInfo.InvariantCulture)
                                         };
                        if (_licenseInfo.Type == 0)
                        {
                            _licenseInfo.ValidDays = Convert.ToInt32(licInfo[3], CultureInfo.InvariantCulture);
                        }
                        _loaded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Could not load license file. " + Environment.NewLine +
                                            "Error:" + ex.Message);
            }
        }
    }
}
