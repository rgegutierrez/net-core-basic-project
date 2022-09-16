using CoreJwtExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJwtExample.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

        Task SendActivationCodeAsync(UserActivationCode request);
    }
}
