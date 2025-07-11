using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repository
{
    public interface IAuditoriumRepository
    {
       Auditorium GetAuditoriumById(int audi_id);
    }
}
