﻿// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections;
using System.Collections.Generic;
using NICE.Foundation;
using NICE.Protocols.Ethernet;

// ReSharper disable InconsistentNaming

namespace NICE.Hardware.Abstraction
{
    public abstract class Device : Stateable, IEnumerable<(string name, EthernetPort port)>
    {
        private readonly Dictionary<string, EthernetPort> _ports = new Dictionary<string, EthernetPort>();

        public readonly string Hostname;
        protected readonly Func<string, Action<EthernetPort>> OnConnect;
        protected readonly Func<string, Action<EthernetPort>> OnDisconnect;
        protected readonly Action OnShutdown;
        protected readonly Action OnStartUp;

        protected Action<EthernetFrame, EthernetPort> OnReceive;

        protected Device(string hostname, Action<EthernetFrame, EthernetPort> onReceive, Action onStartUp = null, Action onShutdown = null, Func<string, Action<EthernetPort>> onConnect = null, Func<string, Action<EthernetPort>> onDisconnect = null)
        {
            Hostname = hostname;

            if (onReceive == null)
            {
                OnReceive = (frame, port) => { };
            }
            else
            {
                OnReceive = onReceive;
            }

            if (onConnect == null)
            {
                OnConnect = s => { return ethernetPort => { }; };
            }
            else
            {
                OnConnect = onConnect;
            }

            if (onDisconnect == null)
            {
                OnDisconnect = s => { return ethernetPort => { }; };
            }
            else
            {
                OnDisconnect = onDisconnect;
            }

            if (onStartUp == null)
            {
                OnStartUp = () => { };
            }
            else
            {
                OnStartUp = onStartUp;
            }

            if (onShutdown == null)
            {
                OnShutdown = () => { };
            }
            else
            {
                OnShutdown = onShutdown;
            }

            if (Global.Devices.ContainsKey(Hostname))
            {
                throw new Exception("A device with the same hostname already exists!");
            }

            Global.Devices[Hostname] = this;
        }

        //If the user wants devices to auto startup => Global.SetDeviceAutoStartUp(true)
        //If the user wants ports to auto init => Global.SetPortAutoInit(true)
        public bool PowerOn { get; private set; }

        protected void PostConstruct()
        {
            if (Global.DeviceAutoStartup)
            {
                StartUp();
            }
        }
        
        public EthernetPort this[string name]
        {
            get
            {
                if (_ports.ContainsKey(name))
                {
                    return _ports[name];
                }

                Log.Debug(Hostname, $"Creating new port {name}...");
                _ports[name] = new EthernetPort(name, this, OnConnect(name), OnDisconnect(name));

                _ports[name].OnReceive(bytes =>
                {
                    if (!PowerOn)
                    {
                        Log.Error(Hostname, "This device is turned off and can't receive frames!");
                        return;
                    }

                    var frame = EthernetFrame.FromBytes(bytes);
                    OnReceive(frame, _ports[name]);
                });

                if (Global.PortAutoInit)
                {
                    _ports[name].Init();
                }
                
                return _ports[name];
            }
        }

        public IEnumerator<(string name, EthernetPort port)> GetEnumerator()
        {
            foreach (var (key, value) in _ports)
            {
                yield return (key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void StartUp()
        {
            Log.Info(Hostname, "Device is starting up...");
            OnStartUp();
            PowerOn = true;
            Log.Debug(Hostname, "Device start up complete!");
        }

        public void Shutdown()
        {
            Log.Info(Hostname, "Device is shutting down...");
            OnShutdown();
            PowerOn = false;
            Log.Debug(Hostname, "Device shutdown complete!");
        }
    }
}