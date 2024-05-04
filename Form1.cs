using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenTK;

namespace Televisor3D
{
    public partial class Form1 : Form
    {
        private Escenario escenario;
        private Dictionary<string, Objeto> objetos = new Dictionary<string, Objeto>();

        Game game;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            escenario = new Escenario(new Vector(0,0,0));
            escenario.Adicionar("Televisor1", ArchivoJson<Objeto>.Cargar("../../Objetos/tele.json"));
            escenario.Adicionar("Jarron1", ArchivoJson<Objeto>.Cargar("../../Objetos/Jarron.json"));
            escenario.Adicionar("Reproductor1", ArchivoJson<Objeto>.Cargar("../../Objetos/Reproductor.json"));
            // escenario = ArchivoJson<Escenario>.Cargar("../../Objetos/escenario.json");
        
            Thread hilo = new Thread(Ejecutar);
            hilo.Start();

            listBox1.Items.Clear();
            listBox1.Items.Add("Escenario");
            foreach (var objeto in escenario.objetos.Keys)
                listBox1.Items.Add(objeto);

            listBox1.SelectedIndex = 0;
            ActualizarInformacionObjetos();
        }
        private void Ejecutar()
        {
            game = new Game(600, 700, "LearnOpenTK");
            game.escenario = this.escenario;
            game.Run();
        }

        private void ActualizarInformacionObjetos()
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedItem.ToString() == "Escenario")
            {
                listBox2.Enabled= false;
                return;
            }
            listBox2.Enabled = true;
            listBox2.Items.Add("Objeto");
            foreach ( var parte in escenario.GetObjeto(listBox1.SelectedItem.ToString()).GetListaDeParte().Keys) 
            {
                listBox2.Items.Add(parte);
            }
            listBox2.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarInformacionObjetos();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vector3 factorEscala = ObtenerFactorEscala();

            if (listBox1.SelectedItem.ToString() == "Escenario")
            {
                escenario.Escalar(factorEscala.X, factorEscala.Y, factorEscala.Z);
            }
            else
            {
                // Obtener el nombre del objeto seleccionado en la listBox1
                string nombreObjeto = listBox1.SelectedItem.ToString();

                // Verificar si el nombre del objeto es válido
                if (escenario.objetos.ContainsKey(nombreObjeto))
                {
                    Objeto objeto = escenario.objetos[nombreObjeto];

                    // Verificar si el objeto seleccionado es un objeto completo o una parte de un objeto
                    if (listBox2.SelectedItem.ToString() == "Objeto")
                    {
                        // Escalar el objeto completo
                        objeto.Escalar(factorEscala.X, factorEscala.Y, factorEscala.Z);
                    }
                    else
                    {
                        // Obtener el nombre de la parte del objeto seleccionada en la listBox2
                        string nombreParte = listBox2.SelectedItem.ToString();

                        // Verificar si la parte seleccionada es válida
                        if (objeto.GetListaDeParte().ContainsKey(nombreParte))
                        {
                            // Escalar la parte del objeto
                            objeto.GetListaDeParte()[nombreParte].Escalar(factorEscala.X, factorEscala.Y, factorEscala.Z);
                        }
                        else
                        {
                            MessageBox.Show("Seleccione una parte válida del objeto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un objeto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private Vector3 ObtenerFactorEscala()
        {
            float factorEscalaX, factorEscalaY, factorEscalaZ;

            // Intenta convertir el texto del cuadro de texto en un número flotante
            if (float.TryParse(textBox1.Text, out factorEscalaX)&& 
                float.TryParse(textBox1.Text, out factorEscalaY) &&
                float.TryParse(textBox1.Text, out factorEscalaZ))
            {
                // Devuelve el factor de escala si la conversión fue exitosa
                return new Vector3(factorEscalaX, factorEscalaY, factorEscalaZ);
            }
            else
            {
                MessageBox.Show("Factor de escala inválido. Por favor ingrese un número válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Vector3.One; // Valor predeterminado de factor de escala
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Vector3 angulosRotacion = ObtenerAngulosRotacion();

            if (listBox1.SelectedItem.ToString() == "Escenario")
            {
                escenario.Rotar(angulosRotacion.X, angulosRotacion.Y, angulosRotacion.Z);
            }
            else
            {
                string nombreObjeto = listBox1.SelectedItem.ToString();
                if (listBox2.SelectedItem.ToString() == "Objeto")
                {
                    escenario.GetObjeto(nombreObjeto).Rotar(angulosRotacion.X, angulosRotacion.Y, angulosRotacion.Z);
                }
                else
                {
                    string nombreParte = listBox2.SelectedItem.ToString();
                    escenario.GetObjeto(nombreObjeto).GetParte(nombreParte).Rotar(angulosRotacion.X, angulosRotacion.Y, angulosRotacion.Z);
                }
            }
        }

        private Vector3 ObtenerAngulosRotacion()
        {
            float anguloX, anguloY, anguloZ;

            // Intenta convertir el texto de los cuadros de texto en números flotantes para los ángulos de rotación en X, Y y Z
            if (float.TryParse(textBox7.Text, out anguloX) &&
                float.TryParse(textBox8.Text, out anguloY) &&
                float.TryParse(textBox9.Text, out anguloZ))
            {
                // Devuelve los ángulos de rotación en X, Y y Z si la conversión fue exitosa
                return new Vector3(anguloX, anguloY, anguloZ);
            }
            else
            {
                MessageBox.Show("Valores de ángulo de rotación inválidos. Por favor ingrese números válidos en los cuadros de texto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Valores predeterminados para los ángulos de rotación si la conversión falla
                return Vector3.Zero;
            }
        }



        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Vector3 traslacion = ObtenerTraslacion();

            if (listBox1.SelectedItem.ToString() == "Escenario")
            {
                escenario.Trasladar(traslacion.X, traslacion.Y, traslacion.Z);
            }
            else
            {
                // Obtener el nombre del objeto seleccionado en la listBox1
                string nombreObjeto = listBox1.SelectedItem.ToString();

                // Verificar si el nombre del objeto es válido
                if (escenario.objetos.ContainsKey(nombreObjeto))
                {
                    Objeto objeto = escenario.objetos[nombreObjeto];

                    // Verificar si el objeto seleccionado es un objeto completo o una parte de un objeto
                    if (listBox2.SelectedItem.ToString() == "Objeto")
                    {
                        // Trasladar el objeto completo
                        objeto.Trasladar(traslacion.X, traslacion.Y, traslacion.Z);
                    }
                    else
                    {
                        // Obtener el nombre de la parte del objeto seleccionada en la listBox2
                        string nombreParte = listBox2.SelectedItem.ToString();

                        // Verificar si la parte seleccionada es válida
                        if (objeto.GetListaDeParte().ContainsKey(nombreParte))
                        {
                            // Trasladar la parte del objeto
                            objeto.GetListaDeParte()[nombreParte].Trasladar(traslacion.X, traslacion.Y, traslacion.Z);
                        }
                        else
                        {
                            MessageBox.Show("Seleccione una parte válida del objeto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un objeto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Vector3 ObtenerTraslacion()
        {
            float traslacionX, traslacionY, traslacionZ;

            // Intenta convertir el texto de los cuadros de texto en números flotantes para las traslaciones en X, Y y Z
            if (float.TryParse(textBox4.Text, out traslacionX) &&
                float.TryParse(textBox5.Text, out traslacionY) &&
                float.TryParse(textBox6.Text, out traslacionZ))
            {
                // Devuelve las traslaciones en X, Y y Z si la conversión fue exitosa
                return new Vector3(traslacionX, traslacionY, traslacionZ);
            }
            else
            {
                MessageBox.Show("Valores de traslación inválidos. Por favor ingrese números válidos en los cuadros de texto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Valor predeterminado para las traslaciones si la conversión falla
                return Vector3.Zero;
            }
        }


    }
}
