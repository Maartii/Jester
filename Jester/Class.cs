using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin(
        package: "maartii.jester",
        name: "JesterPlugin",
        author: "Maartii",
        version: "1.0.0")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        // Add the line below!
        private readonly IEventManager _eventManager;
        // Add the line below!
        private IDisposable _unregister;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
            // Add the line below!
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            // Add the line below!
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger));
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            // Add the line below!
            _unregister.Dispose();
            return default;
        }
    }
}