### Passo a Passo para preparar o projeto para execução.
1 - Criar "cookies.txt" nesse mesmo diretório.
2 - Acessar o youtube por um navegador logado com uma conta google.
3 - Abrir o developer tools ( f12 ), ir na sessão "Network" e interceptar uma requisição para o
    youtube.com.
    Nela terá o Header de Request chamado "Cookie". Basta selecionar o valor dele e colar no arquivo cookies.txt

### Executando
```dotnet run```