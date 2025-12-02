using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body, bool isHtml = false);
        string LoadTemplate(string templateName);
    }

}
