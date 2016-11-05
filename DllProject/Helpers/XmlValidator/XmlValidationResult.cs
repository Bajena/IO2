using System.Collections.Generic;

namespace DllProject.Helpers.XmlValidator
{
    public class XmlValidationResult
    {
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public bool IsValid { get; set; }
        public XmlValidationResult(bool isValid, List<string> errors = null, List<string> warnings = null)
        {
            IsValid = isValid;

            Errors = errors ?? new List<string>();

            Warnings = warnings ?? new List<string>();
        }
    }
}
