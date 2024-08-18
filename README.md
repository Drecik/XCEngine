## 介绍
基于.Net 8，仿照skynet的模式做的一个C# Actor模型

### 运行方式

直接打开XCEngine.sln即可

### XCEngine.Core

  一些封装好的助手类，之所以提成一个Project我是想以后能给客户端用

### XCEngine.Server
  
#### 1. 线程模型
 
线程模型主要参考的是skynet的线程模型做的，目前只实现了一个timer线程和work线程

1. timer线程目前很简单，只是用来定时唤醒work线程查看有没有消息，skynet为什么这么设计，我后续再补充下。另外timer线程后续也会加入定时器功能。
2. work线程主要用来处理actor消息，acotr消息处理方式也是参考的skynet，用了一个全局队列和Actor内部队列，两层的队列来处理，同时skynet为了避免有些Actor饿死的情况会对线程做一些权重，具体可以看代码。
  
#### 2. Actor

Actor主要代码在Actor中，目前实现了基础的Actor功能。需要注意的是Actor.Start的data参数以及Actor.Send的data参数是需要序列化过的，因为Actor模型之间内存是不共享的。

- Actor.Start

  根据类型创建一个Actor，Actor类型是任意的clas类型。另外创建的Actor类型需要提供一个IActorMessageHandler用来处理Actor的消息，具体参考Example里面的Bootstrap代码。

- Actor.Kill

  杀死指定Actor

- Actor.Exit

  把自己退出

- Actor.Send

  向指定Actor发送消息

#### 3. Timer
-  XC.Delay

  延迟一定毫秒数之后执行message id的消息

- XC.Tick

  延迟一定毫秒数之后开始每隔指定毫秒时间执行message id的消息

### XCEngine.Server.Example

XCEngine.Server的一些简单使用例子

## Features
1. ~~简化序列化操作~~
  
    Actor.Start和Actor.Send支持最多5个的自定义传参操作，框架使用的是MemoryPack来实现序列化和反序列化，如果需要替换可以实现ISerializer接口，然后替换Actor.MessageSerializer

2. ~~实现定时器功能~~

    参考文档中的Timer

3. 利用async await实现Actor.Call
4. 实现socket封装
5. 利用属性自动执行消息id回调