using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.FakerContext
{
    public interface IFakerContext
    {
        int GetInt();
        double GetDouble();
        void GetBytes(byte[] buffer);
        Object GetObject(Type type);
    }
}
