using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDK
{
    public interface IScript
    {
        //Guid Id { get; set; }

        void Init();
        void Tick();
        void Exit();
        //void Tick();
    }
}
