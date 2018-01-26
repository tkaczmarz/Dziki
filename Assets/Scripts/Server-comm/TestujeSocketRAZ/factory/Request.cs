using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.factory
{
    interface Request
    {
        bool isRequestSuccess();
        bool CancelConnection();
        String responseMessage();
        String additionalInformations();
    }
}
