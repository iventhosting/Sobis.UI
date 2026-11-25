/* ==========================================================================
   Sobis.UI — Standalone Browser Interop Helpers
   Popover positioning, outside-click, scroll, focus, theme, file download & print.
   ========================================================================== */

(function (global) {
    'use strict';

    // ── Popover / Dropdown Positioning + Outside-Click ──────────────────────
    const popoverRegistry = new Map();

    function positionPopover(root, panelRef) {
        if (!root || !panelRef) return;

        try {
            if (typeof panelRef.showPopover === 'function') {
                panelRef.showPopover();
            }
        } catch (e) { }

        const rect = root.getBoundingClientRect();
        const isFixedWidth = panelRef.classList.contains('sbs-date-panel') || panelRef.hasAttribute('data-sbs-fixed-width');
        panelRef.style.position = 'fixed';
        panelRef.style.left = rect.left + 'px';
        panelRef.style.right = 'auto';
        panelRef.style.width = isFixedWidth ? '' : (rect.width + 'px');
        panelRef.style.margin = '0';
        panelRef.style.zIndex = '999999';
        panelRef.style.maxHeight = '';

        const margin = 8;
        const spaceBelow = global.innerHeight - rect.bottom - margin;
        const spaceAbove = rect.top - margin;
        const panelHeight = panelRef.offsetHeight;
        let openUp = panelRef.classList.contains('sbs-combo-panel-up');

        if (!openUp && panelHeight > spaceBelow && spaceAbove > spaceBelow) {
            openUp = true;
        } else if (openUp && panelHeight > spaceAbove && spaceBelow > spaceAbove) {
            openUp = false;
        }

        if (openUp) {
            panelRef.style.top = 'auto';
            panelRef.style.bottom = (global.innerHeight - rect.top + 4) + 'px';
        } else {
            panelRef.style.top = (rect.bottom + 4) + 'px';
            panelRef.style.bottom = 'auto';
        }

        const available = openUp ? spaceAbove : spaceBelow;
        if (panelHeight > available) {
            panelRef.style.maxHeight = Math.max(120, available) + 'px';
            panelRef.style.overflowY = 'auto';
        }

        const panelWidth = panelRef.offsetWidth;
        if (rect.left + panelWidth > global.innerWidth - margin) {
            panelRef.style.left = Math.max(margin, global.innerWidth - panelWidth - margin) + 'px';
        }
    }

    global.addEventListener('scroll', function (e) {
        if (e.target && typeof e.target.closest === 'function') {
            if (e.target.closest('.sbs-combo-panel') || e.target.closest('.sbs-popover-panel')) {
                return;
            }
        }

        popoverRegistry.forEach(function (entry) {
            try {
                entry.dotNetRef.invokeMethodAsync(entry.callbackMethod);
            } catch (err) { }
        });
    }, true);

    const popover = {
        attach: function (root, dotNetRef, panelRef, callbackMethod) {
            if (!root || popoverRegistry.has(root)) return;

            const method = callbackMethod || 'OnOutsideClick';
            const listener = function (e) {
                if (!root.contains(e.target) && (!panelRef || !panelRef.contains(e.target))) {
                    try {
                        dotNetRef.invokeMethodAsync(method);
                    } catch (err) { }
                }
            };

            setTimeout(function () {
                document.addEventListener('mousedown', listener);
            }, 0);

            popoverRegistry.set(root, {
                dotNetRef: dotNetRef,
                listener: listener,
                panel: panelRef,
                callbackMethod: method
            });

            if (panelRef) {
                positionPopover(root, panelRef);
            }
        },

        reposition: function (root) {
            const entry = popoverRegistry.get(root);
            if (entry && entry.panel) {
                positionPopover(root, entry.panel);
            }
        },

        detach: function (root) {
            const entry = popoverRegistry.get(root);
            if (!entry) return;

            document.removeEventListener('mousedown', entry.listener);
            if (entry.panel) {
                try {
                    if (typeof entry.panel.hidePopover === 'function') {
                        entry.panel.hidePopover();
                    }
                } catch (e) { }
            }
            popoverRegistry.delete(root);
        }
    };

    // ── Generic Outside-Click Helper (Drawers, Menus, Tooltips) ─────────────
    const outsideClickRegistry = new Map();

    const clickOutside = {
        attach: function (element, dotNetRef, callbackMethod) {
            if (!element || outsideClickRegistry.has(element)) return;

            const method = callbackMethod || 'OnOutsideClick';
            const listener = function (e) {
                if (!element.contains(e.target)) {
                    try {
                        dotNetRef.invokeMethodAsync(method);
                    } catch (err) { }
                }
            };

            setTimeout(function () {
                document.addEventListener('mousedown', listener);
            }, 0);

            outsideClickRegistry.set(element, listener);
        },

        detach: function (element) {
            const listener = outsideClickRegistry.get(element);
            if (!listener) return;
            document.removeEventListener('mousedown', listener);
            outsideClickRegistry.delete(element);
        }
    };

    // ── Scroll Helpers ──────────────────────────────────────────────────────
    const savedScrollPositions = {};

    const scroll = {
        toBottom: function (element) {
            if (element) {
                element.scrollTop = element.scrollHeight;
            }
        },

        toElement: function (elementId) {
            const element = document.getElementById(elementId);
            if (element && typeof element.scrollIntoView === 'function') {
                element.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        },

        rowIntoView: function (selector) {
            const el = document.querySelector(selector);
            if (el && typeof el.scrollIntoView === 'function') {
                el.scrollIntoView({ block: 'nearest', inline: 'nearest' });
            }
        },

        savePosition: function (containerSelector) {
            const container = document.querySelector(containerSelector);
            if (container) {
                savedScrollPositions[containerSelector] = {
                    x: container.scrollLeft,
                    y: container.scrollTop
                };
            }
        },

        restorePosition: function (containerSelector) {
            const pos = savedScrollPositions[containerSelector];
            if (pos) {
                const container = document.querySelector(containerSelector);
                if (container) {
                    setTimeout(function () {
                        container.scrollTo({ left: pos.x, top: pos.y, behavior: 'instant' });
                    }, 50);
                }
            }
        }
    };

    // ── Focus & Keyboard Helpers ────────────────────────────────────────────
    const focus = {
        element: function (el) {
            if (el && typeof el.focus === 'function') {
                try { el.focus(); } catch (e) { }
            }
        },

        byId: function (id) {
            const el = document.getElementById(id);
            if (el && typeof el.focus === 'function') {
                try { el.focus(); } catch (e) { }
            }
        },

        guardArrowKeys: function (id) {
            const el = document.getElementById(id);
            if (!el || el.dataset.sbsArrowGuard) return;
            el.dataset.sbsArrowGuard = '1';
            el.addEventListener('keydown', function (e) {
                if (e.key === 'ArrowUp' || e.key === 'ArrowDown') {
                    e.preventDefault();
                }
            });
        }
    };

    // ── Theme Helpers ───────────────────────────────────────────────────────
    const theme = {
        get: function () {
            try {
                return localStorage.getItem('sobis-theme') === 'light' ? 'light' : 'dark';
            } catch (e) {
                return 'dark';
            }
        },

        set: function (value) {
            const normalized = value === 'light' ? 'light' : 'dark';
            try {
                localStorage.setItem('sobis-theme', normalized);
            } catch (e) { }

            if (normalized === 'light') {
                document.documentElement.setAttribute('data-theme', 'light');
            } else {
                document.documentElement.removeAttribute('data-theme');
            }

            if (global.chrome && global.chrome.webview) {
                try {
                    global.chrome.webview.postMessage(JSON.stringify({ type: 'TEMA_DEGISTI', theme: normalized }));
                } catch (e) { }
            }

            return normalized;
        },

        toggle: function () {
            return theme.set(theme.get() === 'light' ? 'dark' : 'light');
        },

        init: function () {
            return theme.set(theme.get());
        }
    };

    // ── File Download & HTML Print Helpers ──────────────────────────────────
    const file = {
        downloadBytes: function (fileName, contentType, byteArray) {
            const blob = new Blob([byteArray], { type: contentType });
            const url = URL.createObjectURL(blob);
            const anchorElement = document.createElement('a');
            anchorElement.href = url;
            anchorElement.download = fileName || '';
            document.body.appendChild(anchorElement);
            anchorElement.click();
            anchorElement.remove();
            setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
        }
    };

    const print = {
        html: function (htmlContent, title) {
            const printFrame = document.createElement('iframe');
            printFrame.style.position = 'fixed';
            printFrame.style.right = '0';
            printFrame.style.bottom = '0';
            printFrame.style.width = '0';
            printFrame.style.height = '0';
            printFrame.style.border = '0';
            document.body.appendChild(printFrame);

            const printDocument = printFrame.contentWindow.document;
            printDocument.open();
            printDocument.write('<html><head><title>' + (title || '') + '</title>');
            printDocument.write('<style>' +
                '@page { size: A4 landscape; margin: 0; }' +
                'html, body { margin: 0; padding: 0; font-family: sans-serif; font-size: 11px; }' +
                'table.print-shell { width: 100%; border-collapse: collapse; border: 0; }' +
                'table.print-shell > thead > tr > td, table.print-shell > tfoot > tr > td { border: 0; padding: 0; height: 10mm; }' +
                'table.print-shell > tbody > tr > td { border: 0; padding: 0 10mm; }' +
                'table.print-shell table { width: 100%; border-collapse: collapse; }' +
                'table.print-shell table thead { display: table-header-group; }' +
                'table.print-shell table tr { break-inside: avoid; page-break-inside: avoid; }' +
                'table.print-shell table th, table.print-shell table td { border: 1px solid #ddd; padding: 6px 8px; text-align: left; }' +
                'table.print-shell table th { background-color: #f2f2f2; }' +
                '</style>');
            printDocument.write('</head><body>');
            printDocument.write('<table class="print-shell"><thead><tr><td></td></tr></thead><tbody><tr><td>');
            printDocument.write(htmlContent || '');
            printDocument.write('</td></tr></tbody><tfoot><tr><td></td></tr></tfoot></table>');
            printDocument.write('</body></html>');
            printDocument.close();

            setTimeout(function () {
                printFrame.contentWindow.focus();
                printFrame.contentWindow.print();
                setTimeout(function () { printFrame.remove(); }, 1000);
            }, 300);
        }
    };

    // ── Public Namespace ────────────────────────────────────────────────────
    global.sobisUI = {
        popover: popover,
        clickOutside: clickOutside,
        scroll: scroll,
        focus: focus,
        theme: theme,
        file: file,
        print: print
    };

    // ── Backward-Compatible Global Aliases (preserve existing host overrides if loaded) ──
    if (!global.sbsComboOutsideClick) {
        global.sbsComboOutsideClick = popover;
    }
    if (!global.sbsScrollRowIntoView) {
        global.sbsScrollRowIntoView = scroll.rowIntoView;
    }
    if (!global.sbsOkTuslariImlecKoru) {
        global.sbsOkTuslariImlecKoru = focus.guardArrowKeys;
    }
    if (!global.downloadFileFromBytes) {
        global.downloadFileFromBytes = file.downloadBytes;
    }
    if (!global.printHtml) {
        global.printHtml = print.html;
    }
    if (!global.scrollInterop) {
        global.scrollInterop = {
            scrollToBottom: scroll.toBottom,
            scrollToElement: scroll.toElement,
            saveScrollPosition: scroll.savePosition,
            restoreScrollPosition: scroll.restorePosition
        };
    }
    if (!global.sobisTheme) {
        global.sobisTheme = {
            get: theme.get,
            set: theme.set,
            toggle: theme.toggle
        };
    }
})(window);
