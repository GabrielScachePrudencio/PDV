using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using PDV_LANCHES.Views;
using ServidorLanches.model;

public class Status_Categorias
{
    private static Status_Categorias _instancia;
    private static readonly object _lock = new();

    private readonly HomeAdministrativoController _homeAdministrativoController;

    public List<CategoriaProduto> CategoriaProdutos { get; private set; }
    public List<TipoStatusPedido> TipoStatusPedido { get; private set; }
    public List<FormaDePagamento> FormaDePagamentos { get; private set; }
    public List<CupomDesconto> CuponsDesconto { get; private set; }
    public string teste { get; private set; }

    //GetAllCuponsDesconto
    private bool _carregado = false;

    private Status_Categorias()
    {
        _homeAdministrativoController = new HomeAdministrativoController();
        CategoriaProdutos = new();
        TipoStatusPedido = new();
        FormaDePagamentos = new();
        CuponsDesconto = new();
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

        var todosTipos  = await _homeAdministrativoController.getAllStatus();
        if(todosTipos != null)
        {
            TipoStatusPedido = todosTipos.Where(p => p.ativo).ToList();
        }


        var todasCategorias = await _homeAdministrativoController.getAllCategoria();
        if(todasCategorias != null)
        {
            CategoriaProdutos = todasCategorias.Where(p => p.ativo).ToList();
        }


        var pagamentos = await _homeAdministrativoController.getAllFormasDePagamentos();
        if (pagamentos != null)
        {
            FormaDePagamentos = pagamentos.Where(p => p.Ativo).ToList(); // Apenas as ativas
        }


        var cuponsDeconto = await _homeAdministrativoController.GetAllCuponsDesconto();
        if (cuponsDeconto != null)
        {
            CuponsDesconto = cuponsDeconto.Where(p => p.Ativo).ToList(); // Apenas as ativas
        }

        _carregado = true;
    }

}
