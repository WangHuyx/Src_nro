using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRO.Core
{
    public interface MonoBehaviour
    {
         void Start();
         void FixedUpdate();
         void OnGUI(SKCanvas canvas);
         //void checkInput();
    }
}
