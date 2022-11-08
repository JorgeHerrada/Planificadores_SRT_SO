using ListasTipoCola1;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Planificadores_SRT_SO
{
    public partial class Form1 : Form
    {
        int contadorProcesos = 0;
        int minutos = 0;
        int segundos = 0;
        int barraActual = 0;
        int duracionGlobal = 0;
        bool iniciado = false;
        //bool moveNext = true;
        bool terminado = false;
        int maximoProcesos = 7;
        Cola cola = new Cola();
        int quantum = 2;
        int contadorQuantum = 0;
        bool[] procesoActivo = { false, false, false, false, false, false, false };

        public Form1()
        {
            InitializeComponent();
        }

        private void btnAgregarProceso_Click(object sender, EventArgs e)
        {
            // Validaciones de entradas
            if (txtNombreNuevoProceso.Text == "" || txtDuracionNuevoProceso.Text == "")
            {
                Console.WriteLine("Asigna nombre y duracion al proceso.");
                return;
            }
            else if (!txtDuracionNuevoProceso.Text.All(char.IsDigit))
            {
                Console.WriteLine(txtDuracionNuevoProceso.Text + " IS NOT A VALID INTEGER.");
                return;
            }

            // Se ha alcanzado el limite de procesos? (7 procesos)
            if (maximoProcesos == contadorProcesos)
            {
                MessageBox.Show("Se ha alcanzado el número máximo de procesos.");
                txtDuracionNuevoProceso.Text = "";
                txtNombreNuevoProceso.Text = "";
                return;
            }

            // Aumenta contador de procesos y asigna a la cola
            contadorProcesos++;
            cola.Insertar(contadorProcesos.ToString(), txtNombreNuevoProceso.Text, txtDuracionNuevoProceso.Text);


            // declaramos proceso activo en registrode banderas
            procesoActivo[contadorProcesos - 1] = true;


            // Agregamos a la list box
            lstNumeroProceso.Items.Add(contadorProcesos);
            lstNombreProceso.Items.Add(txtNombreNuevoProceso.Text);
            lstDuracionProceso.Items.Add(txtDuracionNuevoProceso.Text);


            // limpiar entradas de texto
            txtNombreNuevoProceso.Text = "";
            txtDuracionNuevoProceso.Text = "";
        }


        private void btnLimpiarCola_Click(object sender, EventArgs e)
        {
            // detiene timer
            timer.Enabled = false;

            // Elimina todos lo elementos de la cola 
            while (lstNumeroProceso.Items.Count > 0)
            {
                cola.Extraer();
                lstNumeroProceso.Items.RemoveAt(0);
                lstNombreProceso.Items.RemoveAt(0);
                lstDuracionProceso.Items.RemoveAt(0);
            }

            // permite la entrada de nuevos procesos
            txtDuracionNuevoProceso.Enabled = true;
            txtNombreNuevoProceso.Enabled = true;
            btnAgregarProceso.Enabled = true;
            btnIniciarProcesos.Enabled = true;
            btnPausarProcesos.Enabled = false;

            // reinicia contador de procesos y barra de progreso actual
            contadorProcesos = 0;
            barraActual = 0;

            // oculta todas etiquetas, reiniciar y ocultar las barras de progreso 
            lblProceso1.Visible = false;
            lblProceso2.Visible = false;
            lblProceso3.Visible = false;
            lblProceso4.Visible = false;
            lblProceso5.Visible = false;
            lblProceso6.Visible = false;
            lblProceso7.Visible = false;
            lblGlobal.Visible = false;

            pbr1.Visible = false;
            pbr2.Visible = false;
            pbr3.Visible = false;
            pbr4.Visible = false;
            pbr5.Visible = false;
            pbr6.Visible = false;
            pbr7.Visible = false;
            pgrGlobal.Visible = false;

            pbr1.Value = 0;
            pbr2.Value = 0;
            pbr3.Value = 0;
            pbr4.Value = 0;
            pbr5.Value = 0;
            pbr6.Value = 0;
            pbr7.Value = 0;
            pgrGlobal.Value = 0;

            // reiniciar tiempo y ocultar timer
            minutos = segundos = 0;
            lblTimer.Visible = false;
            iniciado = false;
            terminado = false;
            duracionGlobal = 0;
            contadorQuantum = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblTimer.Text = "";
            btnPausarProcesos.Enabled = false;
        }

        private void btnIniciarProcesos_Click(object sender, EventArgs e)
        {
            // Bloquea la entrada de nuevos procesos o reinicio y permite pausar 
            txtDuracionNuevoProceso.Enabled = false;
            txtNombreNuevoProceso.Enabled = false;
            btnAgregarProceso.Enabled = false;

            btnIniciarProcesos.Enabled = false;
            btnPausarProcesos.Enabled = true;

            // checar que lista no esté vacía
            if (lstNumeroProceso.Items.Count == 0)
            {
                return;
            }

            // Inicialización o reanudación?
            if (iniciado)
            {
                timer.Enabled = true;
            }
            else
            {
                // activamos barra global
                pgrGlobal.Visible = true;
                lblGlobal.Visible = true;

                // asignamos su máximo como la suma de la duración de todos los procesos
                for (int i = 0; i < lstNumeroProceso.Items.Count; i++)
                {
                    duracionGlobal += Int32.Parse((string)lstDuracionProceso.Items[i]);
                }
                pgrGlobal.Maximum = duracionGlobal;


                // Activamos etiqueta y barra de progreso del primer proceso
                lblProceso1.Text = (string)lstNombreProceso.Items[0];
                lblProceso1.Visible = true;
                pbr1.Visible = true;
                //currentProcess = 0; // asigna el proceso actual

                // asigna la duraccion del proceso como maximo de la barra de progreso
                pbr1.Maximum = Int32.Parse((string)lstDuracionProceso.Items[0]);

                // Inicializa el timer
                timer.Enabled = true;
                lblTimer.Visible = true;
            }
            // Bandera de que ya se ha hecho la primera inicializacion 
            iniciado = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblBarraActual.Text = "Proceso en curso: " + (barraActual + 1);

            // procesos terminados?
            if (terminado)
            {
                return;
            }

            // formato del timer
            segundos++;
            if (segundos == 60)
            {
                segundos = 0;
                minutos++;
            }
            // mostrar datos del timer
            lblTimer.Text = String.Format("{0:00}:", minutos) + String.Format("{0:00}", segundos);


            // el quantum se completo? cambiamos proceso y reseteamos contadorQuantum
            if (contadorQuantum == quantum)
            {
                cambiarProceso();
            }

            // aumenta el contador de quantum
            contadorQuantum++;


            // avanzamos en contador global
            if (pgrGlobal.Value < duracionGlobal)
            {
                pgrGlobal.Value++;
            }

            switch (barraActual)
            {
                case 0:
                    // Aun no termina el proceso?
                    if (pbr1.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        // aunmenta un paso
                        pbr1.Value++;
                    }
                    else
                    {
                        // se completo el proceso
                        // entonces intenta pasar al siguiente
                        // Y marca bandera de que no hay proceso activo
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        // No hay un siguiente? entonces termina la llamada de la funcion
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 1:
                    pbr2.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso2.Text = (string)lstNombreProceso.Items[barraActual];
                    pbr2.Visible = true;
                    lblProceso2.Visible = true;
                    if (pbr2.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr2.Value++;
                    }
                    else
                    {
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 2:
                    lblProceso3.Visible = true;
                    pbr3.Visible = true;
                    pbr3.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso3.Text = (string)lstNombreProceso.Items[barraActual];
                    if (pbr3.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr3.Value++;
                    }
                    else
                    {
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 3:
                    pbr4.Visible = true;
                    lblProceso4.Visible = true;
                    pbr4.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso4.Text = (string)lstNombreProceso.Items[barraActual];
                    if (pbr4.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr4.Value++;
                    }
                    else
                    {
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 4:
                    pbr5.Visible = true;
                    lblProceso5.Visible = true;
                    pbr5.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso5.Text = (string)lstNombreProceso.Items[barraActual];
                    if (pbr5.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr5.Value++;
                    }
                    else
                    {
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 5:
                    pbr6.Visible = true;
                    lblProceso6.Visible = true;
                    pbr6.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso6.Text = (string)lstNombreProceso.Items[barraActual];
                    if (pbr6.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr6.Value++;
                    }
                    else
                    {
                        try
                        {
                            procesoActivo[barraActual] = false;
                            cambiarProceso();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
                case 6:
                    pbr7.Visible = true;
                    lblProceso7.Visible = true;
                    pbr7.Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                    lblProceso7.Text = (string)lstNombreProceso.Items[barraActual];
                    if (pbr7.Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
                    {
                        pbr7.Value++;
                    }
                    else
                    {
                        procesoActivo[barraActual] = false;
                        cambiarProceso();
                        //timer.Enabled = false;
                    }
                    break;
                default:
                    timer.Enabled = false;
                    break;
            }
        }

        private void btnPausarProcesos_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            btnPausarProcesos.Enabled = false;
            btnIniciarProcesos.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Enabled = false;
        }

        private void cambiarProceso()
        {
            // Se completaron los procesos? paramos timer
            if (procesosTermiados())
            {
                timer.Enabled = false;
                return;
            }

            // extrae informacion de la cola y la agrega al final
            string[] datos = cola.Extraer();
            cola.Insertar(datos[0], datos[1], datos[2]);
            if (barraActual == lstNumeroProceso.Items.Count - 1)
            {
                barraActual = 0;
            }
            else
            {
                barraActual++;
            }
            contadorQuantum = 0;

            // sigue cambiando de proceso hasta que encuentre uno activo
            if (!procesoActivo[barraActual])
            {
                cambiarProceso();
            }
        }

        // revisa si queda algun proceso sin terminar
        // true == TODOS LOS PROCESOS ESTAN TERMINADOS
        private bool procesosTermiados()
        {
            // revisa cada bandera
            for (int i = 0; i < lstNumeroProceso.Items.Count; i++)
            {
                // si hay procesos activos, retorna falso
                if (procesoActivo[i])
                {
                    return false;
                }
            }
            // no hay procesos activos, true
            return true;
        }
    }
}