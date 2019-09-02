using System.Threading;
using System.Threading.Tasks;
using System;

async Task MainAsync() {
    try
    {
        // assign local variable (in vs-code this supresses any errors)
        LightingConsole skm = console;
        // go to 10.0 and print result
        await skm.SelectBlk(10.0);
        // create 10.1 - SKMScript
        await skm.CreateScene("SKMScript", 10.1);

        // first, we have to wait for the intensities to be sent to the client
        Thread.Sleep(250);
        for (int i = 1; i <= 12; i++)
        {
            skm.Stromkreise[i].Intensity = (byte)(i * 21.25);
        }
        // this will send all differences to the skm
        await skm.PushChanges();
        // this adds a scene without selecting it
        await skm.EditScene("Edit", 11.0);
        // now, we're selecting 11.0
        await skm.SelectBlk(11.0);
    }catch(Exception e) {
        LogInfo(e);
    }
}
MainAsync().Wait();
CloseScript();
