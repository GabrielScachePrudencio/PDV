using PDV_LANCHES.controller;
using PDV_LANCHES.model;

public class Status_Categorias
{
    private static Status_Categorias _instancia;
    private static readonly object _lock = new();

    private readonly HomeController _homeController;

    public List<CategoriaProduto> CategoriaProdutos { get; private set; }
    public List<TipoStatusPedido> TipoStatusPedido { get; private set; }

    private bool _carregado = false;

    private Status_Categorias()
    {
        _homeController = new HomeController();
        CategoriaProdutos = new();
        TipoStatusPedido = new();
    }

    public static Status_Categorias Instancia
    {
        get
        {
            lock (_lock)
            {
                _instancia ??= new Status_Categorias();
                return _instancia;
            }
        }
    }

    public async Task CarregarAsync()
    {
        if (_carregado) return;

        TipoStatusPedido = await _homeController.getAllStatus();
        CategoriaProdutos = await _homeController.getAllCategoria();

        _carregado = true;
    }

    public void Invalidar()
    {
        _carregado = false;
        CategoriaProdutos.Clear();
        TipoStatusPedido.Clear();
    }
}
