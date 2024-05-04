using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Televisor3D
{
    class Objeto
    {
        public Dictionary<string, Parte> partes;
        public Vector Centro;

        public Objeto(Dictionary<string, Parte> parte, Vector centro)
        {
            Centro= centro;
            if (parte == null)
            {
                // Manejar el caso en que el diccionario es null
                partes = new Dictionary<string, Parte>();
            }
            else
            {
                // Si el diccionario no es null, asignar el diccionario pasado como parámetro al campo partes
                this.partes = parte;
            }

            foreach (var centros in partes)
            {
                centros.Value.Centro = Centro + centros.Value.GetCentro();
            }
        }

        public void Adicionar(string key, Parte verticeAdicionar)
        {
            partes.Add(key, verticeAdicionar);
        }

        public void Eliminar(string key, Parte verticeEliminar)
        {
            partes.Remove(key);
        }

        public void SetCentro(Vector centro)
        {
            this.Centro = centro;
        }

        public Vector GetCentro()
        {
            return Centro;
        }

        public Parte GetParte(string key)
        {
            return partes[key];
        }

        public Dictionary<String, Parte> GetListaDeParte()
        {
            return partes;
        }

        public void Rotar(double x, double y, double z)
        {
            foreach (var parte in partes)
            {
                parte.Value.Rotar(x, y, z);
            }
        }

        public void Trasladar(double x, double y, double z)
        {
            foreach (var parte in partes)
            {
                parte.Value.Trasladar(x, y, z);
            }
        }

        public void Escalar(double x, double y, double z)
        {
            foreach (var parte in partes)
            {
                parte.Value.Escalar(x, y, z);
            }
        }

        public void Dibujar(Vector centroME)
        {
            var centroMO = this.Centro + centroME;
                foreach (var parte in partes)
                {
                  parte.Value.Dibujar(centroMO);
                }
            
        }

        public void Dibujar(Graphics g)
        {
            var centroMO = this.Centro;
            foreach (var parte in partes)
            {
                parte.Value.Dibujar(centroMO);
            }

        }
    }
}
