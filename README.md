# NeoLog: A Newly Minted Logging Facility for High-Performance Computing

Choosing a best-of-breed logging package or library can be difficult. There are multiple choices, from aged (Log4Net, NSpring, NLog) to newer (Serilog). Logging libraries for .NET tend to suffer from high overhead from object creation and resource contention, with some exceptions. The Neolog library is a complete rewrite of the NSpring logging library, long the fastest library available (and possibly as of this writing, excluding Neolog) though it was written in .NET 2.

It's often a wise practice to isolate a major codebase from third-party details to aid in overall decoupling; it's not unusual to see logging facades created for this reason. In part, the Neolog library itself is a facade. Just as it can write to or adapt multiple outputs (from basics like the file system to log aggregation and team communication facilities like Splunk and Slack), it can adapt other legacy libraries as well. Your code will benefit from greater clarity and abstraction, as well as better performance, whether you use NeoLog's output facilities directly or in facade mode. 

Features of the NeoLog logging system include:
* Written from the ground up in .NET Core
* Incredibly fast performance with low object overhead (best of any .NET logging package, bar none)
* Easy configuration, with support for hierarchical configuration and JSON, XML, and YAML files as well as programmatic configuration
* A simpler, more intuitive API than other logging packages for .NET
* The ability to add logging to any existing class with a marker interface and/or attributes
* Extensible architecture with a wide array of outputs
* Relative stack traces (instead of showing build directories, runtime directories)
* Structured data logging
* Pluggable data formatting including JSON, XML and binary serialization
* The ability to seamlessly coexist with existing logging libraries in a large codebase
* Permissive MIT software license

Features intentionally left out of NeoLog:
* [Fluent interface](https://ocramius.github.io/blog/fluent-interfaces-are-evil/) for configuration
  * This avoids the inherent drawbacks of fluent interfaces, including cluttered API and broken OOP design
* Separate async logging method calls
  * In NeoLog, log methods return instantaneously by default without the need for the programmer to determine what's going on underneath the covers. Synchronous logging is supported via configuration where necessary

Principles embraced by this project:
* Extreme high performance
  * Avoidance of object creation and destruction overhead
    * Use of non-object types (arrays, structs) where helpful
    * Object reuse, e.g. log-entry buffers used by loggers
* Intuitive API design
  * Where possible while satisfying the need for high performance, object oriented principles have been followed. So, for example, log entries/events are implemented as structs, preserving object-like semantics while avoiding object creation and destruction overhead. 
* Easy, flexible configuration
  * Inheritable logger configurations
  * Per-level configurations
* Ease of integration and deployment
  * Minimization of dependencies and library scatter
  * Can be used by deploying a single DLL at minimum
* Security
* Extensibility
* Asynchronicity by default
* Platform agnosticism

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites / Installation

The code can be built and deployed on any .NET Core host platform, including not only Windows but many popular Linux distributions as well (CentOS, Debian, Fedora, Mint, Ubuntu, etc.). The code can be included in your project in source form and/or as built DLLs, and is also available as a NuGet package. 

## Deployment

There are no special requirements for deploying the libraries, except of course to deploy any dependencies as well. So, for example, if NeoLog is used in whole or in part to front-end a legacy, slower logging library like Log4Net, you would include not only NeoLog.dll and NeoLog.Log4Net.dll, but the Log4Net libraries in your deployment as well. It would NOT be necessary to deploy bridge libraries for back-ending logging libraries not used (NeoLog.Serilog.dll if Serilog were not used, for example).

## Built With

* [JSON.NET](https://www.newtonsoft.com/json) - Used for serializing structured object data to JSON

## Contributing

Please read [CONTRIBUTING.md](https://github.com/NeoLog) for details on joining the project.

## Authors

* **Jeffrey Varszegi** - *Initial work* - [NeoLog](https://github.com/NeoLog)

(See also the list of contributors who participate(d) in this project.)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* The NSpring library served as partial inspiration for this project; its focus on avoiding unnecessary overhead reportedly keeps it in use in high-performance systems to this day
