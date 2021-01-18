using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.ClassFolder
{
    class PtrPage
    {
        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public extern static int DocumentProperties(
            IntPtr hWnd,               //handle to parent window 
            IntPtr hPrinter,           // handle to printer object
            string pDeviceName,   // device name
            ref IntPtr pDevModeOutput, // modified device mode
            ref IntPtr pDevModeInput,   // original device mode
            int fMode);                 // mode options 

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv")]
        public static extern int PrinterProperties(
            IntPtr hwnd,  // handle to parent window
            IntPtr hPrinter); // handle to printer object

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public extern static int OpenPrinter(
           string pPrinterName,   // printer name
           ref IntPtr hPrinter,      // handle to printer object
           ref IntPtr pDefault);    // handle to default printer object. 

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public static extern int ClosePrinter(
            IntPtr phPrinter); // handle to printer object

        /*public static void GetdocumentProp()
        {
            string printerName = _document.PrinterSettings.PrinterName;

            if (printerName != null && printerName.Length > 0)
            {
                IntPtr pPrinter = IntPtr.Zero;
                IntPtr pDevModeOutput = IntPtr.Zero;
                IntPtr pDevModeInput = IntPtr.Zero;
                IntPtr nullPointer = IntPtr.Zero;

                OpenPrinter(printerName, ref pPrinter, ref nullPointer);

                int iNeeded = DocumentProperties(this.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, 0);
                pDevModeOutput = System.Runtime.InteropServices.Marshal.AllocHGlobal(iNeeded);
                DocumentProperties(this.Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, DM_PROMPT);
                ClosePrinter(pPrinter);
            }
        }

        public static void GetprintProp()
        {
            string printerName = _document.PrinterSettings.PrinterName;

            if (printerName != null && printerName.Length > 0)
            {
                IntPtr pPrinter = IntPtr.Zero;
                IntPtr pDevModeOutput = IntPtr.Zero;
                IntPtr pDevModeInput = IntPtr.Zero;
                IntPtr nullPointer = IntPtr.Zero;

                OpenPrinter(printerName, ref pPrinter, ref nullPointer);

                int iNeeded = PrinterProperties(this.Handle, pPrinter);
                ClosePrinter(pPrinter);
            }
        }*/
    }
}
