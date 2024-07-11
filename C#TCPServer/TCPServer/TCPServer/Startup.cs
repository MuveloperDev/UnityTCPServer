using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace TCPServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Configure, ConfigureServices 함수는 ASP.NET Core의 실행 과정 중 자동으로 호출.

        #region [ Simple Echo Server ]
        /*
         * ConfigureServices 메소드는 의존성 주입(Dependency Injection) 컨테이너에 서비스를 등록하기 위해 사용됩니다. 
         * 이 메소드는 호스트가 구성되고 애플리케이션이 시작될 때 호출됩니다. 
         * ASP.NET Core는 시작 시 Startup 클래스를 인스턴스화하고, 
         * 이 클래스 내의 ConfigureServices 메소드를 찾아 실행하여 
         * 애플리케이션의 서비스를 DI 컨테이너에 등록합니다.
         * 
         * ConfigureServices와 Configure는 Main 메소드에서 CreateHostBuilder를 통해 
         * 설정된 IHostBuilder의 Build 메소드가 호출될 때 실행되는 프로세스의 일부입니다. 
         */
        public void ConfigureServices(IServiceCollection services)
        {
            // 기본 서비스 추가.
            /* 라우팅 기능을 추가합니다.
             * 이는 엔드 포인트 라우팅을 활성화하여 요청을 적절한 핸들러로 라우팅할 수 있게 해준다.
             */
            services.AddRouting();
        }
        /* Configure 메소드는 HTTP 요청 파이프라인을 설정합니다. 
         * 이 메소드는 ConfigureServices 다음에 호출되며, 주로 미들웨어를 구성하는 데 사용됩니다. 
         * 이 메소드 내에서 미들웨어를 순서대로 등록하면, 
         * 요청이 들어올 때 각 미들웨어를 통과하게 됩니다.
         */
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /* 1. 개발 환경에서는 개발자 예외 페이지를 사용하여 오류 발생 시 자세한 정보를 제공한다.
             * 2. UsingRouting과 UseEndpoints를 사용하여 요청 라우팅을 설정합니다.
             * - 여기서 / 경로로 들어오는 WebSocket 요청을 처리하고,
             * WebSocket 요청이 아닌 경우 400을 처리합니다.
             * 3. UseWebSocket을 호출하여 애플리케이션 WebSocket을 사용할 수 있도록 설정합니다.
             * WebSocketOptions를 통해 WebSocket의 KeepAliveInterval과 ReceiveBuffSize를 설정합니다
             */
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                });
            });

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            });
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            /* 1. 실제 WebSocket 메시지 처리 로직을 담당합니다.
             * 2. 클라이언트로부터 메시지를 받아 동일한 메시지를 에코로 다시 전송합니다 연결이 닫힐 때까지 이과정을 반복하고,
             * 연결이 닫히면 클라이언트와 WebSocket연결을 종료합니다.
             */
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        #endregion

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    //// WebSocketHandler와 CustomWebSocketHandler 등록
        //    //services.AddSingleton<CustomWebSocketHandler>();


        //    //services.AddControllers();
        //    //services.AddSingleton<BattleRoomMgr>();
        //    //services.AddSingleton<LogManager>();
        //    //services.AddSingleton<SessionMgr>();
        //    //services.AddSingleton<MatchServerAgent>();
        //    //services.AddHttpContextAccessor();
        //    //// SendMessageDelegate를 DI 컨테이너에 등록
        //    //services.AddSingleton<Room.SendMessageDelegate>(provider =>
        //    //{
        //    //    var webSocketHandler = provider.GetRequiredService<CustomWebSocketHandler>();
        //    //    return new Room.SendMessageDelegate((socketId, message) =>
        //    //    {
        //    //        return webSocketHandler.SendMessageAsync(socketId, message);
        //    //    });
        //    //});

        //    // IDistributedCache를 사용하기 위해 메모리 캐시를 DI 컨테이너에 등록
        //    services.AddDistributedMemoryCache();

        //    // 세션을 사용하기 위해 필요
        //    services.AddSession(); services.AddWebSockets(options =>
        //    {
        //        options.KeepAliveInterval = TimeSpan.FromSeconds(120);

        //    });
        //    services.AddTransient<ISessionStore, DistributedSessionStore>();


        //}
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    app.UseSession();
        //    app.UseWebSockets();

        //    app.UseMiddleware<WebSocketMiddleware>();

        //    app.UseRouting();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapGet("/", async context =>
        //        {
        //            await context.Response.WriteAsync("WebSocket server is running.");
        //        });
        //    });

        //    //LogManager.Instance.Log("WebSocket Server started");
        //}
    }
}
