using CoreJwtExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJwtExample.IRepository
{
    public interface IStudentRepository
    {
        Task<List<Student>> Gets1();
        Task<List<Student>> Gets2();
    }
}
