using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Koenig.Maestro.Web.ReactConfig), "Configure")]

namespace Koenig.Maestro.Web
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddV8();
		}
	}
}