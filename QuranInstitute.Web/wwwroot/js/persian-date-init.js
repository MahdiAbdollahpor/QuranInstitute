// ═══════════════════════════════════════════════════════
// تبدیل تاریخ شمسی ↔ میلادی — بدون کتابخانه خارجی
// ═══════════════════════════════════════════════════════
// الگوی استفاده در HTML:
//   <input type="text" class="persian-date-input"
//          data-target="hiddenId"
//          placeholder="1403/08/12"
//          value="1403/08/12" />
//   <input type="hidden" name="StartDate" id="hiddenId"
//          value="2024-11-02" />
//
// کاربر فقط عدد شمسی تایپ می‌کند (با کیبورد فارسی یا انگلیسی).
// در blur، خودکار تبدیل به میلادی در hidden می‌شود.
// ═══════════════════════════════════════════════════════
(function () {
    'use strict';

    // ═══════════════════════════════════════════════
    // الگوریتم تبدیل تقویم (بر اساس jalaali-js)
    // ═══════════════════════════════════════════════

    var breaks = [
        -61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210,
        1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178
    ];

    function div(a, b) { return ~~(a / b); }
    function mod(a, b) { return a - ~~(a / b) * b; }

    function jalCal(jy) {
        var bl = breaks.length,
            gy = jy + 621,
            leapJ = -14,
            jp = breaks[0],
            jm, jump, leap, leapG, march, n, i;

        for (i = 1; i < bl; i += 1) {
            jm = breaks[i];
            jump = jm - jp;
            if (jy < jm) break;
            leapJ = leapJ + div(jump, 33) * 8 + div(mod(jump, 33), 4);
            jp = jm;
        }

        n = jy - jp;
        leapJ = leapJ + div(n, 33) * 8 + div(mod(n, 33) + 3, 4);
        if (mod(jump, 33) === 4 && jump - n === 4) leapJ += 1;

        leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150;
        march = 20 + leapJ - leapG;

        if (jump - n < 6) n = n - jump + div(jump + 4, 33) * 33;
        leap = mod(mod(n + 1, 33) - 1, 4);
        if (leap === -1) leap = 4;

        return { leap: leap, gy: gy, march: march };
    }

    function g2d(gy, gm, gd) {
        var d = div((gy + div(gm - 8, 6) + 100100) * 1461, 4) +
            div(153 * mod(gm + 9, 12) + 2, 5) +
            gd - 34840408;
        d = d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752;
        return d;
    }

    function d2g(jdn) {
        var j, i, gd, gm, gy;
        j = 4 * jdn + 139361631;
        j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
        i = div(mod(j, 1461), 4) * 5 + 308;
        gd = div(mod(i, 153), 5) + 1;
        gm = mod(div(i, 153), 12) + 1;
        gy = div(j, 1461) - 100100 + div(8 - gm, 6);
        return { gy: gy, gm: gm, gd: gd };
    }

    function j2d(jy, jm, jd) {
        var r = jalCal(jy);
        return g2d(r.gy, 3, r.march) + (jm - 1) * 31 -
            div(jm, 7) * (jm - 7) + jd - 1;
    }

    function d2j(jdn) {
        var gy = d2g(jdn).gy,
            jy = gy - 621,
            r = jalCal(jy),
            jdn1f = g2d(gy, 3, r.march),
            jd, jm, k;

        k = jdn - jdn1f;
        if (k >= 0) {
            if (k <= 185) {
                jm = 1 + div(k, 31);
                jd = mod(k, 31) + 1;
                return { jy: jy, jm: jm, jd: jd };
            } else {
                k -= 186;
            }
        } else {
            jy -= 1;
            k += 179;
            if (r.leap === 1) k += 1;
        }
        jm = 7 + div(k, 30);
        jd = mod(k, 30) + 1;
        return { jy: jy, jm: jm, jd: jd };
    }

    function toJalaali(gy, gm, gd) {
        return d2j(g2d(gy, gm, gd));
    }

    function toGregorian(jy, jm, jd) {
        return d2g(j2d(jy, jm, jd));
    }

    // ─── API عمومی ───
    function jalaaliToGregorianStr(jy, jm, jd) {
        var g = toGregorian(jy, jm, jd);
        return g.gy + '-' + pad(g.gm) + '-' + pad(g.gd);
    }

    function gregorianStrToJalaali(gregorianStr) {
        if (!gregorianStr) return null;
        var parts = gregorianStr.split('-');
        if (parts.length !== 3) return null;
        var gy = parseInt(parts[0], 10);
        var gm = parseInt(parts[1], 10);
        var gd = parseInt(parts[2], 10);
        if (isNaN(gy) || isNaN(gm) || isNaN(gd)) return null;
        var j = toJalaali(gy, gm, gd);
        return { jy: j.jy, jm: j.jm, jd: j.jd };
    }

    function pad(n) { return (n < 10 ? '0' : '') + n; }

    // ═══════════════════════════════════════════════
    // تبدیل اعداد فارسی/عربی به انگلیسی
    // ═══════════════════════════════════════════════
    function toEnglishDigits(s) {
        if (s == null) return '';
        var fa = '۰۱۲۳۴۵۶۷۸۹';
        var ar = '٠١٢٣٤٥٦٧٨٩';
        s = String(s);
        for (var i = 0; i < 10; i++) {
            s = s.split(fa[i]).join(i).split(ar[i]).join(i);
        }
        return s;
    }

    // ═══════════════════════════════════════════════
    // اعتبارسنجی تاریخ شمسی
    // ═══════════════════════════════════════════════
    function isValidShamsi(jy, jm, jd) {
        if (jy < 1300 || jy > 1500) return false;
        if (jm < 1 || jm > 12) return false;
        if (jd < 1 || jd > 31) return false;

        // ماه‌های 1-6: 31 روز
        if (jm <= 6 && jd > 31) return false;
        // ماه‌های 7-11: 30 روز
        if (jm >= 7 && jm <= 11 && jd > 30) return false;
        // ماه 12 (اسفند): 29 یا 30 روز (بسته به کبیسه)
        if (jm === 12) {
            var r = jalCal(jy);
            var maxDay = r.leap === 0 ? 30 : 29;
            if (jd > maxDay) return false;
        }

        return true;
    }

    // ═══════════════════════════════════════════════
    // تبدیل کامل شمسی → میلادی (از رشته)
    // ═══════════════════════════════════════════════
    function shamsiStringToGregorian(shamsiStr) {
        if (!shamsiStr) return null;

        var clean = toEnglishDigits(shamsiStr).trim().replace(/-/g, '/');
        var parts = clean.split('/');

        if (parts.length !== 3) return null;

        var jy = parseInt(parts[0], 10);
        var jm = parseInt(parts[1], 10);
        var jd = parseInt(parts[2], 10);

        if (isNaN(jy) || isNaN(jm) || isNaN(jd)) return null;
        if (!isValidShamsi(jy, jm, jd)) return null;

        return jalaaliToGregorianStr(jy, jm, jd);
    }

    // ═══════════════════════════════════════════════
    // میلادی → شمسی (رشته)
    // ═══════════════════════════════════════════════
    function gregorianStringToShamsi(gregorianStr) {
        var j = gregorianStrToJalaali(gregorianStr);
        if (!j) return '';
        return j.jy + '/' + pad(j.jm) + '/' + pad(j.jd);
    }

    // ═══════════════════════════════════════════════
    // راه‌اندازی یک input
    // ═══════════════════════════════════════════════
    function initOne(input) {
        if (input.dataset.pdpInitialized === '1') return;
        input.dataset.pdpInitialized = '1';

        var targetId = input.dataset.target;
        var hidden = targetId
            ? document.getElementById(targetId)
            : input.parentElement.querySelector('input[type="hidden"]');

        // ─── هماهنگی اولیه ───
        if (hidden && hidden.value && !input.value) {
            input.value = gregorianStringToShamsi(hidden.value);
        }

        // ─── تبدیل لحظه‌ای اعداد فارسی → انگلیسی ───
        input.addEventListener('input', function () {
            var pos = input.selectionStart;
            var original = input.value;
            var converted = toEnglishDigits(original);

            if (converted !== original) {
                input.value = converted;
                try {
                    input.setSelectionRange(pos, pos);
                } catch (e) { /* بی‌اهمیت */ }
            }
        });

        // ─── در blur، تبدیل قطعی ───
        input.addEventListener('blur', function () {
            var typed = toEnglishDigits(input.value).trim();

            if (!typed) {
                input.value = '';
                if (hidden) hidden.value = '';
                return;
            }

            var gregorian = shamsiStringToGregorian(typed);

            if (gregorian) {
                if (hidden) hidden.value = gregorian;

                // نمایش با فرمت یکسان (yyyy/MM/dd)
                var clean = typed.replace(/-/g, '/');
                var parts = clean.split('/');
                var jy = parseInt(parts[0], 10);
                var jm = parseInt(parts[1], 10);
                var jd = parseInt(parts[2], 10);
                input.value = jy + '/' + pad(jm) + '/' + pad(jd);

                // نشانه موفقیت
                input.style.borderColor = '';
            } else {
                // نامعتبر
                input.style.borderColor = '#b91c1c';
            }
        });

        // ─── در focus، رنگ خطا را پاک کن ───
        input.addEventListener('focus', function () {
            input.style.borderColor = '';
        });

        // ─── Enter → blur ───
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                input.blur();
            }
        });
    }

    function initAll(root) {
        root = root || document;
        root.querySelectorAll('.persian-date-input').forEach(initOne);
    }

    window.initPersianDatePickers = initAll;

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initAll();
        });
    } else {
        initAll();
    }
})();