namespace WebApiCanciones.Services
{
    public class Actividad3 : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Act3.txt";
        private Timer timer;

        public Actividad3 (IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(ConcatenaMensajeFecha, null, TimeSpan.Zero, TimeSpan.FromSeconds(120)); //Cada cuando tiempo se imprime en el archivo
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }

        private void ConcatenaMensajeFecha(object state)
        { 
            EscribeMensaje("El Profe Gustavo Rodriguez es el mejor " + DateTime.Now.ToString("hh:mm:ss"));
        }

        private void EscribeMensaje(string msg)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(msg); }

        }
    }
}
