# Writting Events

## Handle Events

Complex events that need to be broken down are written with 
>	`internal void HandleXEvent( xEventData )`

The internal Handler is responsible for processing the Raw data.  Handlers should not be public

### Example
A good example is `HandlePointerEvent` in AsciiApp.
This function will pass the event to the topmost window for processing, and if not handled, then any lower windows.
The window also has a HandlePointerEvent where it will deal with changing focus, clicks, and so-on.


## Subclasses and Subscribing Events

In order to support extending a class to extend functionality of an Event or
simply subscribing to an event from an existing object, we need two options.

The first is having an event handler defined for other code to subscribe to.
>	`public event EventHandler<XEventData> XEvent;`

The second is a virtual OnXEvent function which can be overwritten.  By default, it should trigger the event handler
>	`virtual void OnXEvent(xEventData) => XEvent?.Invoke(this, xEventData);`


## Raise Events

Triggering events can only be invoked from the class declaring the event.  To raise the event, a RaiseXEvent function is required.

>	`protected virtual void RaiseXEvent(xEventData) => XEvent?.Invoke(this, xEventData);`

If the event is only triggered in the class that defines them, then the RaiseXEvent function is optional.

