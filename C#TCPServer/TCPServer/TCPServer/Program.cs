using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using TCPServer;

namespace UnityWebServer
{
    public class Program
    {
        public static string environmentName = "mumuWeb";
        public static void Main(string[] arg)
        {

            /* 기본 호스트 빌더를 생성하고 반환.
             * Build()
             * 1. IHostBuilder에서 설정된 모든 구성 요소와 서비스, 
             * 설정을 기반으로 IHost 인스턴스를 생성합니다. 
             * 2. 이 과정에서 구성된 모든 서비스 
             * (예: 데이터베이스 커넥션, 파일 시스템 서비스 등)가 DI 컨테이너에 등록되며, 
             * 애플리케이션 실행을 위한 모든 준비가 마무리됩니다.
             * 
             * Run()
             * 1. 구축된 IHost 인스턴스를 실행한다.
             * 2. 애플리케이션을 시작
             * 3. 내부적으로 애플리케이션의 생명주기와 요청 파이프라인을 관리하는 웹 서버를 활성화한다.
             * 4. Run이 호출되면 서버는 포트를 열고 들어오는 HTTP 요청을 수신하기 시작한다.
             */
            CreateHostBuilder(arg).Build().Run();
        }

        /*IHostBuilder 인스턴스를 생성하는 정적 메소드.
         * Host.CreateDefaultBuilder(args)의 역할
         * 1. 기본 호스트 빌더를 생성하고 반환.
         * 2. 애플리케이션의 설정 파일 (appsettings.json, 환경 변수 기반 설정 파일 등) 로드.
         * 3.기본 로깅 설정 (콘솔, 디버그, 등에 로그 출력).
         * 4. 기본적인 의존성 주입 (DI) 컨테이너 설정.
         * 5. 개발 환경에서는 개발자 환경에 특화된 설정을 로드*/
        public static IHostBuilder CreateHostBuilder(string[] args)
         => Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostBuildContext, config) =>
         {
             // ConfigureAppConfiguration 는 애플리케이션의 구성을 세부적으로 조정하는데 사용.
             /*
              * hostBuildContext.HostingEnvironment
              * 현재 호스팅 환경의 정보를 가져옵니다. 
              * 이 변수를 사용하여 개발, 스테이징, 프로덕션 등 다양한 환경 설정을 구분할 수 있습니다.
              */
             var env = hostBuildContext.HostingEnvironment;
             /*
              * config.SetBasePath(Directory.GetCurrentDirectory());
              * 구성 파일을 로드할 때 참조할 기준 경로를 현재 작업 디렉토리로 설정
              */
             config.SetBasePath(Directory.GetCurrentDirectory());

             //if (false == string.IsNullOrEmpty(environmentName))
             //{ 
             //   var specificSetting
             //}


             /*
              * ConfigureWebHostDefaults는 웹 애플리케이션을 쉽게 구성하고, 
              * 관리하기 위한 기본 설정들을 제공하며,
              * 웹 호스트의 세부적인 설정을 캡슐화하여 
              * 개발자가 웹 애플리케이션에 더 집중할 수 있도록 도와줍니다.
              */
         }).ConfigureWebHostDefaults(webBuilder => {
             /*
              * webBuilder.UseStartup<Startup>();
              * 
              * 웹 애플리케이션의 설정을 담당하는 Startup 클래스를 지정합니다. 
              * Startup 클래스는 애플리케이션의 서비스 설정, 미들웨어 파이프라인 구성 등을 담당합니다. 
              * 이러한 설정은 웹 애플리케이션의 핵심 구성 부분이므로, 
              * ConfigureWebHostDefaults 메소드 안에서 반드시 지정해야 합니다.
              */
             webBuilder.UseStartup<Startup>();
         });
    }
}